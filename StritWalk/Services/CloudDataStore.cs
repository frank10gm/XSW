using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Plugin.Settings;
using Xamarin.Forms;

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
                if(start == 0)
                {
					var locator = CrossGeolocator.Current;
					locator.DesiredAccuracy = 100;
					var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

                    Settings.lat = position.Latitude.ToString().Replace(",", ".");
                    Settings.lng = position.Longitude.ToString().Replace(",", ".");
                }

				var contentType = "application/json";
                var req = "{\"action\":\"getPosts\", \"num\":\""+start+"\", \"order\":\"added\", \"order2\":\"20\", \"lat\":\""+Settings.lat+"\", \"lng\":\""+Settings.lng+"\", \"user_id\":\"1\" }";
				var httpContent = new StringContent(req, Encoding.UTF8, contentType);

                //var json = await client.GetStringAsync($"api/item");
                var resp = await client.PostAsync($"", httpContent);

                var json = await resp.Content.ReadAsStringAsync();
                if(json == "[]")
                {
                    Settings.listEnd = true;
                    MessagingCenter.Send<CloudDataStore, bool>(this, "NotEnd", false);
                }
                items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
            }

            return items;
        }

		public async Task<IList<Item>> GetMapItemsAsync(bool forceRefresh = false, int start = 0)
		{
			if (forceRefresh && CrossConnectivity.Current.IsConnected)
			{
				if (start == 0)
				{
					var locator = CrossGeolocator.Current;
					locator.DesiredAccuracy = 100;
					var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

					Settings.lat = position.Latitude.ToString().Replace(",", ".");
					Settings.lng = position.Longitude.ToString().Replace(",", ".");
				}

				var contentType = "application/json";
				var req = "{\"action\":\"getPosts\", \"num\":\"" + start + "\", \"order\":\"added\", \"order2\":\"10000\", \"lat\":\"" + Settings.lat + "\", \"lng\":\"" + Settings.lng + "\", \"user_id\":\"1\" }";
				var httpContent = new StringContent(req, Encoding.UTF8, contentType);

				//var json = await client.GetStringAsync($"api/item");
				var resp = await client.PostAsync($"", httpContent);

				var json = await resp.Content.ReadAsStringAsync();
				if (json == "[]")
				{
					Settings.listEnd = true;
				}
                Console.WriteLine(json);
				items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
			}

			return items;
		}

        public async Task<Item> GetItemAsync(string id)
        {
            if (id != null && CrossConnectivity.Current.IsConnected)
            {
				//var json = await client.GetStringAsync($"api/item/{id}");
				//items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
				//var contentType = "application/json";
				//var req = "{\"action\":\"getPosts\", \"num\":\"" + start + "\", \"order\":\"added\", \"order2\":\"20\", \"lat\":\"" + Settings.lat + "\", \"lng\":\"" + Settings.lng + "\", \"user_id\":\"1\" }";
				//var httpContent = new StringContent(req, Encoding.UTF8, contentType);

				//var json = await client.GetStringAsync($"api/item");
				//var resp = await client.PostAsync($"", httpContent);

				//var json = await resp.Content.ReadAsStringAsync();
				//if (json == "[]")
				//{
				//	Settings.listEnd = 1;
				//}
				//items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
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
                if((int)ao[0]["data"] != 0){
                    Settings.AuthToken = (string)ao[0]["user_id"];
                    Settings.UserId = (string)ao[0]["user"];
                    result = true;
                }
                //serializzare utente
                //items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
			}
            return result;
		}

        public async Task<bool> Post(string id_user, string name, string audio, string lat, string lng, string description)
        {
			bool result = false;
            if ((!string.IsNullOrEmpty(description) || !string.IsNullOrWhiteSpace(description)) && CrossConnectivity.Current.IsConnected)
			{
				var contentType = "application/json";
                var json = $"{{ action: 'post', id: '{Settings.AuthToken}', name: '', audio: '', lat: '{Settings.lat}', lng: '{Settings.lng}', description: '{description}' }}";
				JObject o = JObject.Parse(json);
				json = o.ToString(Formatting.None);
				var httpContent = new StringContent(json, Encoding.UTF8, contentType);
				var req = await client.PostAsync($"", httpContent);
				var resp = await req.Content.ReadAsStringAsync();
                var ao = JObject.Parse(resp);

                Console.WriteLine(resp);
                result = true;

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
    }
}
