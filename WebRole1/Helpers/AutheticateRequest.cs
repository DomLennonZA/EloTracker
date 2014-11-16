using EloTracker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebRole1.Helpers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthenticateRequestAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                using (EloTrackerEntities context = new EloTrackerEntities())
                {
                    string id = actionContext.Request.Headers.GetValues("gameid").First();
                    Guid gId = Guid.Parse(id);
                    return context.Games.Any(g => g.ID == gId);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}