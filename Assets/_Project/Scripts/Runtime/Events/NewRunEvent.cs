namespace NJG.Runtime.Events
{
    public struct NewRunEvent
    {
        public int LevelSeed { get; }
    
        public NewRunEvent(int levelSeed)
        {
            LevelSeed = levelSeed;
        }
    }
}