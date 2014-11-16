using EloTracker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebRole1.Controllers
{
    public class PlayerController : ApiController
    {
        public MatchHistory[] GetMatchHistory(Guid id)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                return context.MatchHistories.Where(m => m.Player1ID == id || m.Player2ID == id).OrderByDescending(m => m.TimeStamp).ToArray();
            }
        }
    }
}
