using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Users
{
    public class PagedUsersRequest
    {
        [Required]
        public int PageIndex { get; set; }
        [Required]
        public int PageSize { get; set; }
        [Required]
        public int SortTypeId { get; set; }

        public string SearchTerm { get; set; } = null;

        public int? RoleId { get; set; } = 0;
    }
}
