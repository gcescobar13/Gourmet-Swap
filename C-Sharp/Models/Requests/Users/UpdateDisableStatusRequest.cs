using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Users
{
    public class UpdateDisableStatusRequest
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool Disabled { get; set; }
    }
}
