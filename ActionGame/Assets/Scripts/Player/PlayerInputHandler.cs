using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : IInputHandler, IDisposable
{
    private PlayerControls _playerControls;
    public event Action OnJumpPressed;

    public PlayerInputHandler()
    {
        _playerControls = new PlayerControls();
        _playerControls.Player.Jump.performed += context => HandleJump(context);
        _playerControls.Enable();
    }

    // public float GetHorizontalInput()
    // {
        //return   //_playerControls.Player.Move.ReadValue<Vector2>().x;
    // }

    private void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpPressed?.Invoke();
        }
    }

    public bool IsJumpPressed()
    {
        return _playerControls.Player.Jump.triggered;
    }

    public void Dispose()
    {
        _playerControls.Disable();
    }
}