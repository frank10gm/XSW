using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Plugin.Settings;

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
                    Settings.listEnd = 1;
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
				var req = "{\"action\":\"getPosts\", \"num\":\"" + start + "\", \"order\":\"added\", \"order2\":\"100\", \"lat\":\"" + Settings.lat + "\", \"lng\":\"" + Settings.lng + "\", \"user_id\":\"1\" }";
				var httpContent = new StringContent(req, Encoding.UTF8, contentType);

				//var json = await client.GetStringAsync($"api/item");
				var resp = await client.PostAsync($"", httpContent);

				var json = await resp.Content.ReadAsStringAsync();
				if (json == "[]")
				{
					Settings.listEnd = 1;
				}
				items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
			}

			return items;
		}

        public async Task<Item> GetItemAsync(string id)
        {
            if (id != null && CrossConnectivity.Current.IsConnected)
            {
                var json = await client.GetStringAsync($"api/item/{id}");
                items = await Task.Run(() => JsonConvert.DeserializeObject<IList<Item>>(json));
            }

            return null;
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
