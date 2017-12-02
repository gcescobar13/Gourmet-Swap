using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Users
{
    public class UserAddRequest
    {
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$", ErrorMessage ="Email is invalid. Format is invalid.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[0-9]).{6,}$", ErrorMessage = "Password must be at least 6 character long and include at least one number")]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^\D?(\d{3})\D?\D?(\d{3})\D?(\d{4})$", ErrorMessage = "Invalid phone number. Requires a US phone number WITH area code.")]
        public string Number { get; set; }
        [Required]
        public string ZipCode { get; set; }
        
        
    }
}
