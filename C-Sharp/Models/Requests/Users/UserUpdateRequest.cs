
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GSwap.Models.Requests.Users
{ 
    public class UserUpdateRequest : UserAddRequest
    {
        public int Id { get; set; }
    }
}