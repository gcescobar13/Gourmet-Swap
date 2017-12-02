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
    public class SearchCuisineByPrice : BaseRequest
    {
        public decimal? Price { get; set; }
        public string[] AddressArray { get; set; }
        public int CuisineId { get; set; }
        public List<MealSearchResultV2> Meals { get; set; }
    }
}
