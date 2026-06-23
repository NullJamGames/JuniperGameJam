using NJG.Runtime.Entity;

namespace NJG.Runtime.Events
{
    public struct AddedModifierEvent
    {
        public BaseModifierSO Modifier { get; private set; }
        
        public AddedModifierEvent(BaseModifierSO modifier)
        {
            Modifier = modifier;
        }
    }
}