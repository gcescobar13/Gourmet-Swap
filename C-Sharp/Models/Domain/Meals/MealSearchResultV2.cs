using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Domain.Meals
{
    [Serializable]
    public class MealSearchResultV2 : MealSearchResult
    {
        
        public int CuisineId { get; set; }
    }
}
