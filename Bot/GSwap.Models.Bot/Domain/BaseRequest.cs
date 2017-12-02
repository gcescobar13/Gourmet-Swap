using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Models.Bot.Domain
{
    [Serializable]
    public class BaseRequest
    {
        public string Location { get; set; }

        public virtual string Key { get { return "searchMealRequest"; } }
        
        public string[] AddressesArray { get; set; }

        public string  Cuisine { get; set; }

    }
}
