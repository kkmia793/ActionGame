using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyCharacter : BaseCharacter
{
    [SerializeField] private CharacterStats enemyStats;
    [SerializeField] private GameObject deathEffectPrefab;

    public event Action<EnemyCharacter> OnDeath; // 死亡通知用のイベント

    protected override void Awake()
    {
        base.Awake();
        if (enemyStats != null)
        {
            ApplyStats(enemyStats);
        }
    }

    private void OnEnable()
    {
        ResetHealth();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (Health <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        PlayDeathEffect();
        OnDeath?.Invoke(this); // イベントを発火
    }

    private void PlayDeathEffect()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void ResetHealth()
    {
        if (enemyStats != null)
        {
            Health = enemyStats.health;
        }
    }

    private void ApplyStats(CharacterStats stats)
    {
        MoveSpeed = stats.moveSpeed;
        Health = stats.health;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
        }
    }

    private void HandlePlayerCollision(Collision2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player == null) return;

        Vector2 contactPoint = collision.GetContact(0).point;
        Vector2 enemyCenter = GetComponent<Collider2D>().bounds.center;

        if (contactPoint.y > enemyCenter.y)
        {
            ApplyBounceToPlayer(player);
            Die();
        }
        else
        {
            player.TakeDamage(enemyStats.attackPower);
        }
    }

    private void ApplyBounceToPlayer(PlayerCharacter player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, enemyStats.jumpForce);
        }
    }
}