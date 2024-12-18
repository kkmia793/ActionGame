using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacter : BaseCharacter, IDamageable, IMovable
{
    private Rigidbody2D _rb;
    private IInputHandler _inputHandler;
    private int _jumpCount = 0;
    private int _maxJumpCount = 2;
    private bool _isGrounded;
    

    [SerializeField] private CharacterStats characterStats;

    public Vector2 CurrentSpeed => _rb.velocity;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();

        if (characterStats != null)
        {
            Health = characterStats.health;
            MoveSpeed = characterStats.moveSpeed;
        }

        _inputHandler = new PlayerInputHandler();
        Initialize(_inputHandler);
    }

    private void Update()
    {
        AutoScroll();
    }
    
    private void AutoScroll()
    {
        _rb.velocity = new Vector2(MoveSpeed, _rb.velocity.y);
    }
    
    public void SetMoveSpeed(float newSpeed)
    {
        MoveSpeed = newSpeed;
    }

    public override void Move(Vector2 direction)
    {
        _rb.velocity = new Vector2(direction.x * MoveSpeed, _rb.velocity.y);
    }

    private void HandleJump()
    {
        if (_jumpCount < _maxJumpCount)
        {
            _rb.velocity = new Vector2(MoveSpeed , characterStats.jumpForce);
            _isGrounded = false;
            _jumpCount++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            _jumpCount = 0;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
        
        if (_inputHandler is PlayerInputHandler playerInputHandler)
        {
            playerInputHandler.OnJumpPressed -= HandleJump;
        }
    }
    
    public void Initialize(IInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
        if (_inputHandler is PlayerInputHandler playerInputHandler)
        {
            playerInputHandler.OnJumpPressed += HandleJump;
        }
    }
}