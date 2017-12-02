using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Email
{
   public class RecoveryRequest
    {
        [Required]
        public string Email { get; set; }

    }
}
