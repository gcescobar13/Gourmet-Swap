using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Domain.Meals
{
    public class Meal
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public int CuisineId {get;set;}

        public string Title { get; set; }

        public string Description { get; set; }
        
        public int DeliveryOption { get; set; }

        public string Comment { get; set; }

        public int DietaryLabel { get; set; }

        public int UserId { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }       
    }
}