using NJG.Runtime.Entity;

namespace NJG.Runtime.Events
{
    public class UpdatedModifierEvent
    {
        public BaseModifierSO Modifier { get; private set; }
        public float RemainingDuration { get; private set; }
        
        public UpdatedModifierEvent(BaseModifierSO modifier, float remainingDuration)
        {
            Modifier = modifier;
            RemainingDuration = remainingDuration;
        }
    }
}