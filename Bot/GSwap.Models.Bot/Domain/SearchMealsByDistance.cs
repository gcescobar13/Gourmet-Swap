using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Bot
{
    [Serializable]
    public class SearchMealsByDistance : SearchMealNameRequest
    {
        public int CuisineId { get; set; }

        public int Distance { get; set; }
    }
}
