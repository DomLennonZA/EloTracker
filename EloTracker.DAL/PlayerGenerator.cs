using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EloTracker.DAL
{
    public static class PlayerGenerator
    {
        private const int BRONZE_ELO_MAX = 2000;
        private const int SILVER_ELO_MAX = 2400;
        private const int BRONZE_MODIFIER = 30;
        private const int SILVER_MODIFIER = 20;
        private const int GOLD_MODIFIER = 10;
        private const string GAME_ID = "b48d5611-b383-49b3-94f1-b3417ceb967e";

        public static void GenerateUsers(bool force = false)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                int count = 0;

                if (context.Players.Count() > 0 && !force)
                {
                    return;
                }
                else if (force)
                {
                    Player[] players = context.Players.ToArray();
                    for (int i = 0; i < players.Length; i++)
                    {
                        context.Players.Remove(players[i]);
                    }
                    context.SaveChanges();
                }

                using (StreamReader sr = new StreamReader("Data\\GamerTags.txt"))
                {
                    string data = sr.ReadToEnd();
                    string[] users = data.Split('\n');
                    foreach (string u in users)
                    {
                        try
                        {
                            string gamerTag = u.Replace(" ", "").Split('=')[1];
                            Player p = new Player
                            {
                                Elo = 800,
                                GamerTag = gamerTag,
                                ID = Guid.NewGuid(),
                                IsOnline = false,
                                GameID = Guid.Parse(GAME_ID)
                            };

                            context.Players.Add(p);
                            count++;
                            context.SaveChanges();
                        }
                        catch
                        {
                            // No valid gamertag
                        }
                    }

                    Debug.WriteLine(count.ToString() + " users added!");
                }
            }
        }

        public static void GenerateMatches(int matchCount)
        {
            using (EloTrackerEntities context = new EloTrackerEntities())
            {
                for (int i = 0; i < matchCount; i++)
                {
                    Player p1 = context.Players.OrderBy(p => Guid.NewGuid()).FirstOrDefault();
                    Guid p2ID = MatchMaking.FindMatch(Guid.Parse(GAME_ID), p1.ID) ?? Guid.Empty;
                    Player p2 = context.Players.OrderBy(p => p.ID == p2ID).FirstOrDefault();
                    
                    if (p2 == null)
                    {
                        i--;
                        continue;
                    }

                    Guid matchID = MatchMaking.CreateMatch(Guid.Parse(GAME_ID), p1.ID, p2.ID);
                    Guid result = IsPlayer1Winner(p1.Elo, p2.Elo) ? p1.ID : p2.ID;
                    MatchMaking.RegisterMatchResult(matchID, p1.ID, result);
                    MatchMaking.RegisterMatchResult(matchID, p2.ID, result);
                }
            }
        }

        // Negate this value for player 2's elo gain/loss
        private static bool IsPlayer1Winner(int player1Elo, int player2Elo)
        {
            // Calculate a winner using the odds
            double odds = 1 / (1 + Math.Pow(10, (player2Elo - player1Elo) / 400));
            return new Random().NextDouble() <= odds;
        }
    }
}
