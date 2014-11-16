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
    public class PlayerController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public Player Info (Guid id)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                return context.Players.FirstOrDefault(p => p.ID == id);
            }
        }

        [HttpGet]
        public MatchHistory[] GetMatchHistory(Guid id, int count = 50)
        {
            return MatchMaking.GetMatchResults(id, count);
        }
    }
}
