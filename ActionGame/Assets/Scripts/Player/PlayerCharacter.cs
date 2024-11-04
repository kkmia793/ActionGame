using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacter : BaseCharacter , IDamageable , IMovable
{
    private Rigidbody2D _rb;
    private IInputHandler _inputHandler;

    [SerializeField] private CharacterStats characterStats;
    private bool _isGrounded;

    protected override void Awake()
    {
        base.Awake();
        
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            enabled = false;
            return;
        }

        if (characterStats != null)
        {
            Health = characterStats.health;
            MoveSpeed = characterStats.moveSpeed;
        }

        _inputHandler = new PlayerInputHandler();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }
    
    public override void Move(Vector2 direction)
    {
        _rb.velocity = new Vector2(direction.x * MoveSpeed, _rb.velocity.y);
    }

    private void HandleMovement()
    {
        float horizontalInput = _inputHandler.GetHorizontalInput();
        Vector2 direction = new Vector2(horizontalInput, 0f);

        if (Mathf.Abs(direction.x) > Mathf.Abs(horizontalInput))
        {
            Move(direction);
        }
    }

    private void HandleJump()
    {
        if (_inputHandler.IsJumpPressed() && _isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, characterStats.jumpForce);
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }


}
