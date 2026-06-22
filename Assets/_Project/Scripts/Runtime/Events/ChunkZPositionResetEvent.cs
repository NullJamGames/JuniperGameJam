namespace NJG.Runtime.Events
{
    public struct ChunkZPositionResetEvent
    {
        public float ZPositionResetAmount { get; private set; }
        
        public ChunkZPositionResetEvent(float zPositionResetAmount)
        {
            ZPositionResetAmount = zPositionResetAmount;
        }
    }
}