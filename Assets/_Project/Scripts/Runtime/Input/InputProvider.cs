using UnityEngine;

namespace NJG.Runtime.Input
{
    public class InputProvider : IInputProvider
    {
        private PlayerInput _playerInput = new();

        public void EnablePlayerInput()
        {
            _playerInput.Player.Enable();
        }

        public void DisablePlayerInput()
        {
            _playerInput.Player.Disable();
        }

        public Vector2 GetMovement()
        {
            return _playerInput.Player.Move.ReadValue<Vector2>();
        }

        public bool WasPausePressed()
        {
            return _playerInput.Player.Pause.WasPressedThisFrame();
        }
    }
}