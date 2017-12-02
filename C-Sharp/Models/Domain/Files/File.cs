using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Domain.Files
{
    public class File
    {
       
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public int FileTypeId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

    }
}
