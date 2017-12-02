using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Comments
{
    public class CommentUpdateRequest : CommentAddRequest
    {
        //
        public int Id { get; set; }
    }
}
