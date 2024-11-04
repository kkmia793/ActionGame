using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : IInputHandler
{
    public float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    public bool IsJumpPressed()
    {
        return Input.GetButtonDown("Jump");
    }
}
