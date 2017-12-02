using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Email
{
    public class EmailRequest
    {
        [Required]
        public string To { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string Subject { get; set; }

    }
}
