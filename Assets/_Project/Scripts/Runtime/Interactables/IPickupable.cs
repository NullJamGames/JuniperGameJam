using NJG.Runtime.Entity;

namespace NJG.Runtime.Interactables
{
    public interface IPickupable
    {
        public void Pickup(IEntity entity);
    }
}