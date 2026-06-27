using UnityEngine;

namespace NJG.Runtime.Input
{
    public interface IInputProvider
    {
        public Vector2 GetMovement();
        public bool WasPausePressed();
    }
}