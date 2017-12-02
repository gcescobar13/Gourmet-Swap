using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Meals
{
    public class MealLocationAddRequest
    {
        [Required]
        public bool CurbPickup { get; set; }

        [Required]        
        public int AddressId { get; set; }

        [Required]
        public int MealId { get; set; }

    }
}
