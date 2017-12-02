using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Domain.Meals
{
    public class MealLocation
    {
        public int Id { get; set; }

        public bool CurbPickup { get; set; }

        public int AddressId { get; set; }

        public int MealId { get; set; }

        public int UserId { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }
    }
}

