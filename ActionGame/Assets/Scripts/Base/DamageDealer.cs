using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // ダメージ量

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                Debug.Log($"Player hit by {gameObject.name}, took {damageAmount} damage.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                Debug.Log($"Player hit by {gameObject.name}, took {damageAmount} damage.");
            }
        }
    }
}