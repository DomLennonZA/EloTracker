using EloTracker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebRole1.Helpers;

namespace WebRole1.Controllers
{
    [AuthenticateRequest]
    public class MatchMakingController : ApiController
    {
        public Guid? FindMatch(Guid playerID)
        {
            return MatchMaking.FindMatch(Request.GetGameID(), playerID);
        }
    }
}
