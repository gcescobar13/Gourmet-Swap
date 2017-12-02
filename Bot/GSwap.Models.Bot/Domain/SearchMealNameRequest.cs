using GSwap.Models.Bot.Domain;
using GSwap.Models.Domain.Meals;
using GSwap.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Bot
{
    [Serializable]
    public class SearchMealNameRequest : BaseRequest
    {

        public string MealName { get; set; }

        public List<MealSearchResultV2> Meals { get; set; }


        public decimal? Price { get; set; }
        
    }
}
