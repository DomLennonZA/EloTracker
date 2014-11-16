using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace WebRole1.Helpers
{
    public static class ExtensionMethods
    {
        public static Guid GetGameID(this HttpRequestMessage req)
        {
            if (!req.Headers.Contains("gameid"))
            {
                return Guid.Empty;
            }
            Guid result = Guid.Empty;
            Guid.TryParse(req.Headers.GetValues("gameid").First(), out result);
            return result;
        }
    }
}