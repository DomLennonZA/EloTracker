using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EloTracker.DAL
{
    public static class MatchMaking
    {
        private const int BRONZE_ELO_MAX = 2000;
        private const int SILVER_ELO_MAX = 2400;
        private const int BRONZE_MODIFIER = 30;
        private const int SILVER_MODIFIER = 20;
        private const int GOLD_MODIFIER = 10;
        private const int MAX_RETRY = 10;
        private const int RETRY_ELO_MODIFIER = 14; // This means the max search band is 140 points diff, putting match odds at ~0.76 either way

        // Using the players Elo we try and find an opponent for the player
        public static Guid? FindMatch(Guid gameID, Guid playerID)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                // TODO: Search for a game, if none found, wait for someone to find me with a timeout
                int banding = MAX_RETRY * RETRY_ELO_MODIFIER;
                Player player = context.Players.FirstOrDefault(p => p.ID == playerID);
                Player opponent = context.Players.Where(p => p.ID != player.ID && p.Elo > player.Elo - banding && p.Elo < player.Elo + banding && p.GameID == gameID).OrderBy(p => p.Elo).ThenBy(p => Guid.NewGuid()).FirstOrDefault();
                if (opponent != null)
                {
                    return opponent.ID;
                }

                return null;
            }
        }

        // Creates a match and returns the match ID
        public static Guid CreateMatch(Guid GameID, Guid p1ID, Guid p2ID)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                MatchHistory match = new MatchHistory
                {
                    GameID = GameID,
                    ID = Guid.NewGuid(),
                    Player1ID = p1ID,
                    Player2ID = p2ID,
                    Player1Verified = false,
                    Player2Verified = false,
                    TimeStamp = DateTime.Now
                };

                context.MatchHistories.Add(match);
                context.SaveChanges();
                return match.ID;
            }
        }

        // We have a two part authentication to make cheating a little harder
        public static void RegisterMatchResult(Guid matchID, Guid playerID, Guid result)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                MatchHistory match = context.MatchHistories.FirstOrDefault(m => m.ID == matchID);

                if (match != null)
                {
                    if (match.WinningPlayerID == null)
                    {
                        match.WinningPlayerID = result;
                    }
                    else if (match.WinningPlayerID != result)
                    {
                        return; // Someone is trying to cheat so dont verify the game
                    }


                    if (match.Player1ID == playerID)
                    {
                        match.Player1Verified = true;
                    }
                    else
                    {
                        match.Player2Verified = true;
                    }

                    context.SaveChanges();

                    // If the match is done, modify elo
                    if (match.Player1Verified == true && match.Player2Verified == true)
                    {
                        Player p1 = context.Players.FirstOrDefault(p => p.ID == match.Player1ID);
                        Player p2 = context.Players.FirstOrDefault(p => p.ID == match.Player2ID);
                        int gameRange = Math.Max(p1.Elo, p2.Elo);
                        int c = BRONZE_MODIFIER;

                        if (gameRange > SILVER_ELO_MAX)
                        {
                            c = GOLD_MODIFIER;
                        }
                        else if (gameRange > BRONZE_ELO_MAX)
                        {
                            c = SILVER_MODIFIER;
                        }

                        double odds = 1 / (1 + Math.Pow(10, (p2.Elo - p1.Elo) / 400));
                        int score = p1.ID == match.WinningPlayerID ? 1 : 0;
                        double mod = p1.Elo - Math.Floor(p1.Elo + c * (score - odds));
                        p1.Elo -= (int)mod;
                        p2.Elo += (int)mod;

                        context.SaveChanges();
                    }
                }
            }
        }

        // Get a list of results for a player
        public static MatchHistory[] GetMatchResults(Guid player, int count = 50)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                return context.MatchHistories.Where(m => m.Player1ID == player || m.Player2ID == player && m.Player1Verified == true && m.Player2Verified == true).OrderByDescending(m => m.TimeStamp).Take(count).ToArray();
            }
        }

        // Clean up dead results
        public static void CleanUp()
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                MatchHistory[] matches = context.MatchHistories.Where(m => m.Player1Verified == false && m.Player2Verified == false).ToArray();
                for (int i = 0; i < matches.Length; i++)
                {
                    context.MatchHistories.Remove(matches[i]);
                    context.SaveChanges();
                }
            }
        }
    }
}