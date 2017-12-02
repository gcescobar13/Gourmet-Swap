using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GSwap.Models.Responses;
using GSwap.Models.Domain.Cuisines;

namespace QnABot.Models
{
    [Serializable]
    public static class GetCuisines
    {

        private static readonly HttpClient client = new HttpClient();

        private static async Task<List<Cuisine>> GetAllCuisines()
        {

            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var client = new HttpClient(handler))
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "http://gswap.azurewebsites.net/api/cooks/cuisineOptions");
                message.Headers.Add("Cookie", "authentication=76-KYpwkkruPX7GXOK_0NmgIxI6WBnT02r8AJk6175nSWAv7-w5p4KUV6xktO9PmUOt1Xv_VZnmj9k10ao6ke_YrosoJAKru-htuu8yu3raT-4jLSHqAXZ_cxjdwyHM8Qov_8qGlmDgQEeaXTc8YL5yH9S_IueaFCSJlMC1ZRxo3zEXPAb7eFmswxd1kcwv3KXYk71ZqfcyygWZhx0lhaYeAEMS5hgqdH3HX1B851vlr5ALJ1hKjPI78ByvxBhZyGtKF7Jj19FgpnwxDlGo_54TQoE1r5GiqbLlBfVR2cH7DesujiLYT5sMcaGNmakPwQBtPSkS8d7rn0fZ1aZ9qDDjVRUlr0lPHv06L2N2dkB2cwaXBURIunRq8_v2aY3HdLghLbEV_UijXQ_D1Wos6q52MQEpn8DU1CmSS6DR6RkXg1WAwxqCof0jZarnO-fnr");

                ItemsResponse<Cuisine> model = null;

                var result = await client.SendAsync(message);
                result.EnsureSuccessStatusCode();
                var jsonString = await result.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<ItemsResponse<Cuisine>>(jsonString);
                return model.Items;

            }
        }

        public async static Task<Dictionary<string, int>> CuisineDictionary()
        {
            List<Cuisine> cuisines = await GetAllCuisines();
            
            Dictionary<string, int> dictCuisine = null;

            

            if (cuisines != null)
            {
                dictCuisine = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

                foreach (var cuisine in cuisines)
                {
                    if (!dictCuisine.ContainsKey(cuisine.Name))
                    {
                        dictCuisine[cuisine.Name] = cuisine.Id;
                    }
                }
            }

            return dictCuisine;

        }
    }
}
