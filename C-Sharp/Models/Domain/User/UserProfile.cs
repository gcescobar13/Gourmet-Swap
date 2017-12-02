using GSwap.Models.Domain.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Domain.Users
{
    [Serializable]
    public class UserProfile: UserLite    
    {
            public string Photo { get; set; } 
    }
}
