using NJG.Runtime.Entity;

namespace NJG.Runtime.Events
{
    public struct AddedModifierEvent
    {
        public Modifier Modifier { get; private set; }
        
        public AddedModifierEvent(Modifier modifier)
        {
            Modifier = modifier;
        }
    }
}