using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Comments
{
    public class CommentAddRequest
    {
        [Required]
        [MaxLength(140)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string Comment { get; set; }
        
        public int? ParentId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int OwnerId { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int OwnerTypeId { get; set; }
        
    }
}
