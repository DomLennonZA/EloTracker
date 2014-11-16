using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EloTracker.DAL
{
    public static class MatchMaking
    {
        private const int MAX_RETRY = 10;
        private const int RETRY_ELO_MODIFIER = 14; // This means the max search band is 140 points diff, putting match odds at ~0.76 either way

        public static Player FindMatch(Player player)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                int banding = MAX_RETRY * RETRY_ELO_MODIFIER;
                Player opponent = context.Players.Where(p => p.ID != player.ID && p.Elo > player.Elo - banding && p.Elo < player.Elo + banding).OrderBy(p => p.Elo).ThenBy(p => Guid.NewGuid()).FirstOrDefault();
                if (opponent != null)
                {
                    return opponent;
                }

                return null;
            }
        }
    }
}