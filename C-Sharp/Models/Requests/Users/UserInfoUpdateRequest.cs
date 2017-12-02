using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Requests.Users
{
    public class UserInfoUpdateRequest : UserInfoAddRequest
    {
        public int Id { get; set; }
    }
}
