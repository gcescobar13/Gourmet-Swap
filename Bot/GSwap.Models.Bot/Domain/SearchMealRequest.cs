using GSwap.Models.Bot.Domain;
using GSwap.Models.Domain.Meals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Bot
{
    [Serializable]
    public class SearchMealRequest : BaseRequest
    {
        //public string[] AddressesArray { get; set; }

        //public string Cuisine { get; set; }

        //public override string Key { get; set; }

        public string MealName { get; set; }

        public List<MealSearchResultV2> Meals { get; set; }

        public decimal? Price { get; set; }

        public int CuisineId { get; set; }

        
    }
}
