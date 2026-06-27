using NJG.Runtime.Entity;

namespace NJG.Runtime.Events
{
    public struct RemovedModifierEvent
    {
        public Modifier Modifier { get; private set; }
        
        public RemovedModifierEvent(Modifier modifier)
        {
            Modifier = modifier;
        }
    }
}