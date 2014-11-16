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
                                IsOnline = false
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
                    Player player1 = context.Players.OrderBy(p => Guid.NewGuid()).FirstOrDefault();
                    Player player2 = MatchMaking.FindMatch(player1);

                    if (player2 == null)
                    {
                        i--;
                        continue;
                    }
                    double p1Diff = CalcualtePlayer1EloDifference(player1.Elo, player2.Elo);

                    MatchHistory history = new MatchHistory
                    {
                        ID = Guid.NewGuid(),
                        Player1ID = player1.ID,
                        Player2ID = player2.ID,
                        TimeStamp = DateTime.Now,
                        WinningPlayerID = p1Diff > 0 ? player1.ID : player2.ID
                    };

                    context.MatchHistories.Add(history);

                    player1.Elo += (int)p1Diff;
                    player2.Elo -= (int)p1Diff;

                    context.SaveChanges();
                }
            }
        }

        // Negate this value for player 2's elo gain/loss
        public static double CalcualtePlayer1EloDifference(int player1Elo, int player2Elo)
        {
            // Calulate the constant for the calculation using the highest Elo player
            int gameRange = Math.Max(player1Elo, player2Elo);
            int c = BRONZE_MODIFIER;

            if (gameRange > SILVER_ELO_MAX)
            {
                c = GOLD_MODIFIER;
            }
            else if (gameRange > BRONZE_ELO_MAX)
            {
                c = SILVER_MODIFIER;
            }

            // Calculate a winner using the odds
            double odds = 1 / (1 + Math.Pow(10, (player2Elo - player1Elo) / 400));
            int score = new Random().NextDouble() <= odds ? 1 : 0;
            return player1Elo - Math.Floor(player1Elo + c * (score - odds));
        }
    }
}
