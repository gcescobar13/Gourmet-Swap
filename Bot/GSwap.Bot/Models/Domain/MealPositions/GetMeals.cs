using GSwap.Models.Domain.Meals;
using GSwap.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace QnABot.Models.MealPositions
{
    [Serializable]
    public class GetMeals : IGetMeals
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<List<MealSearchResultV2>> GetAllMealPositions(double lat, double lng, int radius)
        {

            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var client = new HttpClient(handler))
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, String.Format("http://gswap.azurewebsites.net/api/meals/positions/{0}/{1}/{2}", lat, lng, radius));
                message.Headers.Add("auth", "authentication=76-KYpwkkruPX7GXOK_0NmgIxI6WBnT02r8AJk6175nSWAv7-w5p4KUV6xktO9PmUOt1Xv_VZnmj9k10ao6ke_YrosoJAKru-htuu8yu3raT-4jLSHqAXZ_cxjdwyHM8Qov_8qGlmDgQEeaXTc8YL5yH9S_IueaFCSJlMC1ZRxo3zEXPAb7eFmswxd1kcwv3KXYk71ZqfcyygWZhx0lhaYeAEMS5hgqdH3HX1B851vlr5ALJ1hKjPI78ByvxBhZyGtKF7Jj19FgpnwxDlGo_54TQoE1r5GiqbLlBfVR2cH7DesujiLYT5sMcaGNmakPwQBtPSkS8d7rn0fZ1aZ9qDDjVRUlr0lPHv06L2N2dkB2cwaXBURIunRq8_v2aY3HdLghLbEV_UijXQ_D1Wos6q52MQEpn8DU1CmSS6DR6RkXg1WAwxqCof0jZarnO-fnr");

                ItemsResponse<MealSearchResultV2> model = null;

                HttpResponseMessage result = await client.SendAsync(message);
                //result.EnsureSuccessStatusCode();
                string jsonString = await result.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<ItemsResponse<MealSearchResultV2>>(jsonString);
                return model.Items;

            }
        }

        public async Task<List<MealSearchResultV2>> GetAllMealPositionsByCuisine(double lat, double lng, int radius, int cuisineId)
        {

            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var client = new HttpClient(handler))
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, String.Format("http://gswap.azurewebsites.net/api/meals/positions/{0}/{1}/{2}/{3}", lat, lng, radius, cuisineId));
                message.Headers.Add("Cookie", "authentication=76-KYpwkkruPX7GXOK_0NmgIxI6WBnT02r8AJk6175nSWAv7-w5p4KUV6xktO9PmUOt1Xv_VZnmj9k10ao6ke_YrosoJAKru-htuu8yu3raT-4jLSHqAXZ_cxjdwyHM8Qov_8qGlmDgQEeaXTc8YL5yH9S_IueaFCSJlMC1ZRxo3zEXPAb7eFmswxd1kcwv3KXYk71ZqfcyygWZhx0lhaYeAEMS5hgqdH3HX1B851vlr5ALJ1hKjPI78ByvxBhZyGtKF7Jj19FgpnwxDlGo_54TQoE1r5GiqbLlBfVR2cH7DesujiLYT5sMcaGNmakPwQBtPSkS8d7rn0fZ1aZ9qDDjVRUlr0lPHv06L2N2dkB2cwaXBURIunRq8_v2aY3HdLghLbEV_UijXQ_D1Wos6q52MQEpn8DU1CmSS6DR6RkXg1WAwxqCof0jZarnO-fnr");

                ItemsResponse<MealSearchResultV2> model = null;

                HttpResponseMessage result = await client.SendAsync(message);
                //result.EnsureSuccessStatusCode();
                string jsonString = await result.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<ItemsResponse<MealSearchResultV2>>(jsonString);
                return model.Items;

            }
        }


    }
}