using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour , IDamageable , IMovable
{
    public float Health { get; protected set; }
    public float MoveSpeed { get; protected set; }

    protected virtual void Awake()
    {
        
    }

    public virtual void Move(Vector2 direction)
    {
        
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0) // 死亡処理
        {
            
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public float GetHealth()
    {
        return Health;
    }
}
