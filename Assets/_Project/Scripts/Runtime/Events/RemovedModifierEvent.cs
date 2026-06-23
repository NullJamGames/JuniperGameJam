using NJG.Runtime.Entity;

namespace NJG.Runtime.Events
{
    public struct RemovedModifierEvent
    {
        public BaseModifierSO Modifier { get; private set; }
        
        public RemovedModifierEvent(BaseModifierSO modifier)
        {
            Modifier = modifier;
        }
    }
}