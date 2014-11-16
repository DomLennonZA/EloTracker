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
        [HttpGet]
        public Guid? FindOpponent(Guid id)
        {
            return MatchMaking.FindMatch(Request.GetGameID(), id).Value;
        }

        [HttpPost]
        public Guid CreateMatch(Guid p1ID, Guid p2ID)
        {
            return MatchMaking.CreateMatch(Request.GetGameID(), p1ID, p2ID);
        }

        [HttpPost]
        public void SubmitResult(Guid gameID, Guid playerID, Guid result)
        {
            MatchMaking.RegisterMatchResult(gameID, playerID, result);
        }
    }
}
