using GSwap.Models.TestAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Domain.Comments
{
    public class ThreadMessage
    {
        [EncryptAttr]
        public int Id { get; set; }
        
        public string Title { get; set; }
       
        public string Comment { get; set; }
        
        public int? ParentId { get; set; }
      
        public int OwnerId { get; set; }
      
        public int OwnerTypeId { get; set; }
        
        public int UserId { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }
        
        public List<ThreadMessage> Replies { get; set; }

        public Author AuthorInfo { get; set; }
    }
}
