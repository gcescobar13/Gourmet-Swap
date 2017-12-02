using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Meals
{
    public class MealAddRequest
    {
        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int CuisineId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]        
        public string Description { get; set; }

        [Required]
        public int DeliveryOption { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public int DietaryLabel { get; set; }   
    }
}