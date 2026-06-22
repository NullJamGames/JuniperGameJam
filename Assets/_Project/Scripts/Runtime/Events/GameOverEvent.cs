namespace NJG.Runtime.Events
{
    public struct GameOverEvent
    {
        public int Score { get; private set; }
        
        public GameOverEvent(int score)
        {
            Score = score;
        }
    }
}