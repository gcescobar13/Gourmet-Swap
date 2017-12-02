using GSwap.Models.Domain.Addresses;
using GSwap.Models.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Domain.Meals
{
    [Serializable]
    public class MealSearchResult
    {
        public UserProfile User { get; set; }

        public AddressLite Address { get; set; }

        public int MealId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int DeliveryOption { get; set; }      

        public double Distance { get; set; }

        public string Photo { get; set; }

        public decimal? Price { get; set; }

    }
}