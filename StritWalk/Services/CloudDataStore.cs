using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.IO;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;

namespace StritWalk
{
    public class CloudDataStore : IDataStore<Item>
    {
        HttpClient client;
        IList<Item> items;

        public CloudDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");
            items = new List<Item>();
        }

        public async Task<IList<Item>> GetItemsAsync(bool forceRefresh = false, int start = 0)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                if (start == 0)
                {
                    try
                    {
                        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                        if (status != PermissionStatus.Granted)
                        {
                            if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                            {
                                //Console.WriteLine("### Need location");
                            }

                            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                            //Best practice to always check that the key exists
                            if (results.ContainsKey(Permission.Location))
                                status = results[Permission.Location];
                        }

                        if (status == PermissionStatus.Granted)
                        {
                            Position position = null;
                            try
                            {
                                var locator = CrossGeolocator.Current;
                                locator.DesiredAccuracy = 100;

                                if (position != null)
                                {
                                    //Console.WriteLine("### got cached");
                                }

                                if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled)
                                {
                                    //Console.WriteLine("### geolocator not available");
                                }
                                position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(2000));
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine("### Unable to get location: " + ex);
                            }
                            finally
                            {
                                if (position != null)
                                {
                                    Settings.lat = position.Latitude.ToString().Replace(",", ".");
                                    Settings.lng = position.Longitude.ToString().Replace(",", ".");
                                }
                            }
                        }
                        else if (status != PermissionStatus.Unknown)
                        {
                            //Console.WriteLine("### Location Denied");
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("### Error: " + ex);
                    }
                }

                var contentType = "application/json";
                var req = "{\"action\":\"getPosts\", \"num\":\"" + start + "\", \"order\":\"added\", \"order2\":\"20\", \"lat\":\"" + Settings.lat + "\", \"lng\":\"" + Settings.lng + "\", \"user_id\":\"" + Settings.AuthToken + "\" }";
                var httpContent = new StringContent(req, Encoding.UTF8, contentType);

                //var json = await client.GetStringAsync($"api/item");
                var resp = await client.PostAsync($"", httpContent);

                var json = await resp.Content.ReadAsStringAsync();
                //Debug.WriteLine(json);
                if (json == "[]")
                {
                    Settings.listEnd = true;
                    MessagingCenter.Send<CloudDataStore, bool>(this, "NotEnd", false);
                }

                //Console.WriteLine("### response getposts: " + json);

                items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
            }

            return items;
        }

        public async Task<IList<Item>> GetMapItemsAsync(bool forceRefresh = false, int start = 0)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                var contentType = "application/json";
                var req = "{\"action\":\"getPosts\", \"num\":\"" + start + "\", \"order\":\"added\", \"order2\":\"9999999\", \"lat\":\"" + Settings.lat + "\", \"lng\":\"" + Settings.lng + "\", \"user_id\":\"1\" }";
                var httpContent = new StringContent(req, Encoding.UTF8, contentType);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "");
                requestMessage.Content = httpContent;
                string json = "[]";

                try
                {
                    var resp = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
                    Stream stream = await resp.Content.ReadAsStreamAsync();
                    json = await new StreamReader(stream).ReadToEndAsync();
                }
                catch
                {
                    //Console.WriteLine("@@@ internet connection problem");
                }

                //var resp = await client.PostAsync($"", httpContent);
                //var stream = await resp.Content.ReadAsStreamAsync();
                //string streamreader = await new StreamReader(stream).ReadToEndAsync();
                //Console.WriteLine("###" + streamreader);
                //var json = streamreader;
                //var json = resp.Content.ReadAsStringAsync();  

                if (json == "[]")
                {
                    Settings.listEnd = true;
                }

                try
                {
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

            }

            return items;
        }

        public Task<Item> GetItemAsync(string id)
        {
            if (id != null && CrossConnectivity.Current.IsConnected)
            {

            }

            return null;
        }

        public async Task<bool> Login(string username, string password)
        {
            bool result = false;
            if (username != null && CrossConnectivity.Current.IsConnected)
            {
                var contentType = "application/json";
                var json = $"{{ action: 'login', name: '', user: '{username}', pass: '{password}' }}";
                JObject o = JObject.Parse(json);
                json = o.ToString(Formatting.None);
                var httpContent = new StringContent(json, Encoding.UTF8, contentType);
                var req = await client.PostAsync($"", httpContent);
                var resp = await req.Content.ReadAsStringAsync();
                var ao = JArray.Parse(resp);

                //Console.WriteLine(ao[0]["error"]);
                if ((int)ao[0]["data"] != 0)
                {
                    Settings.AuthToken = (string)ao[0]["user_id"];
                    Settings.UserId = (string)ao[0]["user"];
                    Settings.UserDescription = (string)ao[0]["description"];
                    result = true;
                }
                //serializzare utente
                //items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
            }
            return result;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            if (item == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.PostAsync($"api/item", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode ? true : false;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            if (item == null || item.Id == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);
            var buffer = System.Text.Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);

            var response = await client.PutAsync(new Uri($"api/item/{item.Id}"), byteContent);

            return response.IsSuccessStatusCode ? true : false;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (string.IsNullOrEmpty(id) && !CrossConnectivity.Current.IsConnected)
                return false;

            var response = await client.DeleteAsync($"api/item/{id}");

            return response.IsSuccessStatusCode ? true : false;
        }

        public async Task<string> Post(string id_user, string name, string audio, string lat, string lng, string description)
        {
            string result = string.Empty;
            if ((!string.IsNullOrEmpty(description) || !string.IsNullOrWhiteSpace(description)) && CrossConnectivity.Current.IsConnected)
            {
                var contentType = "application/json";
                var json = $"{{ action: 'post', id: '{Settings.AuthToken}', name: '', audio: '{audio}', lat: '{Settings.lat}', lng: '{Settings.lng}', description: '{description}' }}";
                JObject o = JObject.Parse(json);
                json = o.ToString(Formatting.None);
                var httpContent = new StringContent(json, Encoding.UTF8, contentType);
                var req = await client.PostAsync($"", httpContent);
                var resp = await req.Content.ReadAsStringAsync();
                var ao = JObject.Parse(resp);

                result = (string)ao["new_id"];

                //Console.WriteLine(ao[0]["error"]);
                //if ((int)ao[0]["data"] != 0)
                //{
                //	Settings.AuthToken = (string)ao[0]["user_id"];
                //	Settings.UserId = (string)ao[0]["user"];
                //	result = true;
                //}
                //serializzare utente
                //items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
            }
            return result;
        }

        public async Task<string> SignUp(string username, string password, string email)
        {
            string result = "";
            if (!CrossConnectivity.Current.IsConnected)
            {
                result = "No internet connection.";
                return result;
            }
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email)) return "Fields can't be empty.";
            var user = $"{{ username: '{username}', password: '{password}', mail: '{email}' }}";
            var contentType = "application/json";
            var json = $"{{ action: 'register', user: {user} }}";
            JObject o = JObject.Parse(json);
            json = o.ToString(Formatting.None);
            var httpContent = new StringContent(json, Encoding.UTF8, contentType);
            var req = await client.PostAsync($"", httpContent);
            var resp = await req.Content.ReadAsStringAsync();
            var ao = JObject.Parse(resp);

            if ((int)ao["data"] != 0)
            {
                Settings.AuthToken = (string)ao["user_id"];
                Settings.UserId = (string)ao["username"];
                result = "OK";
            }
            else
            {
                if ((string)ao["error"] == "error_mail") result = "This E-Mail is not available.";
                else if ((string)ao["error"] == "error_username") result = "This Username is already in use.";
            }

            return result;
        }

        public async Task<User> GetMyUser(User me)
        {
            if (!CrossConnectivity.Current.IsConnected) return me;
            
            try
            {
                var contentType = "application/json";
                var json = $"{{ action: 'getMyUser', user: {Settings.AuthToken} }}";
                JObject o = JObject.Parse(json);
                json = o.ToString(Formatting.None);
                var httpContent = new StringContent(json, Encoding.UTF8, contentType);
                var req = await client.PostAsync($"", httpContent);
                var resp = await req.Content.ReadAsStringAsync();
                var ao = JObject.Parse(resp);
                Settings.Num_posts = (int)ao["num_posts"];
                Settings.Num_likes = (int)ao["num_likes"];
                Settings.Num_friends = (int)ao["num_friends"];
                me = await Task.Run(() => JsonConvert.DeserializeObject<User>(resp));
            }
            catch (Exception ex)
            {
                Console.WriteLine("@@@@@@ Exception : " + ex);
            }           

            return me;
        }

        public async Task<int> ILikeThis(string post_id, string action)
        {
            if (!CrossConnectivity.Current.IsConnected) return 1;

            var contentType = "application/json";
            var json = $"{{ action: '{action}', post_id: {post_id}, user_id: {Settings.AuthToken} }}";
            JObject o = JObject.Parse(json);
            json = o.ToString(Formatting.None);
            var httpContent = new StringContent(json, Encoding.UTF8, contentType);
            var req = await client.PostAsync($"", httpContent);
            var resp = await req.Content.ReadAsStringAsync();
            if (resp == "added like.") return 0;
            else if (resp == "removed like.") return 2;
            else return 3;
        }

        public async Task<string> PostComment(string post_id, string comment)
        {
            string result = string.Empty;
            if ((!string.IsNullOrEmpty(comment) || !string.IsNullOrWhiteSpace(comment)) && CrossConnectivity.Current.IsConnected)
            {
                var contentType = "application/json";
                var json = $"{{ action: 'addCommentPost', user_id: '{Settings.AuthToken}', post_id: '{post_id}', comment: '{comment}' }}";
                JObject o = JObject.Parse(json);
                json = o.ToString(Formatting.None);
                var httpContent = new StringContent(json, Encoding.UTF8, contentType);
                var req = await client.PostAsync($"", httpContent);
                var resp = await req.Content.ReadAsStringAsync();
                var ao = JObject.Parse(resp);
                result = (string)ao["new_id"];
            }
            return result;
        }

        public async Task<IList<CommentsItem>> GetComments(string post_id)
        {
            if (!CrossConnectivity.Current.IsConnected) return null;

            var contentType = "application/json";
            var data = $"{{ post_id: '{post_id}' }}";
            var json = $"{{ action: 'getComments', data: {data} }}";
            JObject o = JObject.Parse(json);
            json = o.ToString(Formatting.None);
            var httpContent = new StringContent(json, Encoding.UTF8, contentType);
            var req = await client.PostAsync($"", httpContent);
            var resp = await req.Content.ReadAsStringAsync();
            //var ao = JObject.Parse(resp);        
            //Settings.Num_posts = (int)ao["num_posts"];
            if (resp == "[]")
            {
                MessagingCenter.Send(this, "CommentsEnd", false);
            }
            IList<CommentsItem> commentslist = null;
            try
            {
                commentslist = await Task.Run(() => JsonConvert.DeserializeObject<IList<CommentsItem>>(resp));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return commentslist;
        }

        public async Task<bool> addPushId(string notification_id)
        {
            if (!CrossConnectivity.Current.IsConnected) return false;

            var contentType = "application/json";
            var data = $"{{ user_id: '{Settings.AuthToken}', notification_id: '{notification_id}' }}";
            var json = $"{{ action: 'addPushId', data: {data} }}";
            JObject o = JObject.Parse(json);
            json = o.ToString(Formatting.None);
            var httpContent = new StringContent(json, Encoding.UTF8, contentType);
            var req = await client.PostAsync($"", httpContent);
            var resp = await req.Content.ReadAsStringAsync();
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@ response add push id " + resp);
            var ao = JObject.Parse(resp);
            var result = (string)ao["response"];
            return true;
        }

        public async Task<bool> removePushId()
        {
            if (!CrossConnectivity.Current.IsConnected) return false;

            var contentType = "application/json";
            var data = $"{{ user_id: '{Settings.AuthToken}', notification_id: '{Settings.Notification_id}' }}";
            var json = $"{{ action: 'removePushId', data: {data} }}";
            JObject o = JObject.Parse(json);
            json = o.ToString(Formatting.None);
            var httpContent = new StringContent(json, Encoding.UTF8, contentType);
            var req = await client.PostAsync($"", httpContent);
            var resp = await req.Content.ReadAsStringAsync();
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@ response " + resp);
            var ao = JObject.Parse(resp);
            var result = (string)ao["response"];
            return true;
        }

        public async Task<bool> sendNotifications(string data)
        {
            if (!CrossConnectivity.Current.IsConnected) return false;

            var contentType = "application/json";
            var json = $"{{ action: 'sendNotifications', data: {data} }}";
            JObject o = JObject.Parse(json);
            json = o.ToString(Formatting.None);
            var httpContent = new StringContent(json, Encoding.UTF8, contentType);
            var req = await client.PostAsync($"", httpContent);
            var resp = await req.Content.ReadAsStringAsync();
            Console.WriteLine("@@@@ ask for notification response : " + resp);
            //var ao = JObject.Parse(resp);
            //var result = (string)ao["response"];
            return true;
        }

        public async Task<string> postActivity(string data)
        {
            if (!CrossConnectivity.Current.IsConnected) return "not connected";
            string result = string.Empty;
            var contentType = "application/json";
            var json = $"{{ action: 'postActivity', data: {data} }}";
            JObject o = JObject.Parse(json);
            json = o.ToString(Formatting.None);
            var httpContent = new StringContent(json, Encoding.UTF8, contentType);
            var req = await client.PostAsync($"", httpContent);
            var resp = await req.Content.ReadAsStringAsync();
            //Console.WriteLine("### response " + resp);
            var ao = JObject.Parse(resp);
            result = (string)ao["new_id"];
            return result;
        }


        // upload audio to server
        public async Task<string> UploadAudio(string filePath)
        {
            string result = string.Empty;

            //variable
            var url = "https://www.hackweb.it/api/uploadAudio.php";

            try
            {
                //read file into upfilebytes array
                var upfilebytes = File.ReadAllBytes(filePath);

                //create new HttpClient and MultipartFormDataContent and add our file, and StudentId
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent baContent = new ByteArrayContent(upfilebytes);
                //StringContent studentIdContent = new StringContent("2123");
                if (Device.iOS == Device.RuntimePlatform)
                    content.Add(baContent, "File", "filename.m4a");
                else content.Add(baContent, "File", "filename.m4a");
                //content.Add(studentIdContent, "StudentId");


                //upload MultipartFormDataContent content async and store response in response var
                var response =
                    await client.PostAsync(url, content);

                //read response result as a string async into json var
                var responsestr = response.Content.ReadAsStringAsync().Result;

                //debug
                Debug.WriteLine("response : " + responsestr);
                return responsestr;

            }
            catch (Exception e)
            {
                //debug
                Debug.WriteLine("Exception Caught: " + e.ToString());
                return "error";

            }
            //return result;

        }



    }
}
