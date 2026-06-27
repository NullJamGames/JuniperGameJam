namespace NJG.Runtime.Events
{
    public struct GameOverEvent
    {
        public int Score { get; private set; }
        public int HighScore { get; private set; }
        
        public GameOverEvent(int score, int highScore)
        {
            Score = score;
            HighScore = highScore;
        }
    }
}