namespace NJG.Runtime.Events
{
    public struct ScoreChangedEvent
    {
        public int Score { get; private set; }
        
        public ScoreChangedEvent(int score)
        {
            Score = score;
        }
    }
}