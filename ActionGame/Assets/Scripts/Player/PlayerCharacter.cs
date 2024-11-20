using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacter : BaseCharacter, IDamageable, IMovable
{
    private Rigidbody2D _rb;
    private IInputHandler _inputHandler;
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

    /// <summary>
    /// 自動スクロールを実行する。X方向に一定速度で移動。
    /// </summary>
    private void AutoScroll()
    {
        _rb.velocity = new Vector2(MoveSpeed, _rb.velocity.y);
    }

    /// <summary>
    /// プレイヤーの移動速度を外部から設定。
    /// </summary>
    /// <param name="newSpeed">新しい移動速度。</param>
    public void SetMoveSpeed(float newSpeed)
    {
        MoveSpeed = newSpeed;
    }

    public override void Move(Vector2 direction)
    {
        _rb.velocity = new Vector2(direction.x * MoveSpeed, _rb.velocity.y);
    }

    /// <summary>
    /// ジャンプ操作を処理。
    /// </summary>
    private void HandleJump()
    {
        if (_isGrounded)
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

        // ジャンプ入力ハンドラを解除
        if (_inputHandler is PlayerInputHandler playerInputHandler)
        {
            playerInputHandler.OnJumpPressed -= HandleJump;
        }
    }

    /// <summary>
    /// 入力ハンドラの初期化。
    /// </summary>
    /// <param name="inputHandler">入力ハンドラ。</param>
    public void Initialize(IInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
        if (_inputHandler is PlayerInputHandler playerInputHandler)
        {
            playerInputHandler.OnJumpPressed += HandleJump;
        }
    }
}