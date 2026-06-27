using System;

namespace NJG.Runtime.Online
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string id;
        public string player_name;
        public int score;
        public string ghost_data;
        public int level_seed;
        public string created_at;
    }
}