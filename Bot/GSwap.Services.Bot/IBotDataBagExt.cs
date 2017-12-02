using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSwap.Services.Bot
{
    public static class IBotDataBagExt
    {
        public static void Replace(this IBotDataBag botBag, string key, object val)
        {
            if (botBag.ContainsKey(key))
            {
                botBag.RemoveValue(key);
            }
            botBag.SetValue(key, val);
        }
    }
}
