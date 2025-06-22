using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    private Vector2 cachedKnockbackDir;

    public void SetKnockbackDirection(Vector2 direction)
    {
        cachedKnockbackDir = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthPlayer player = collision.GetComponent<HealthPlayer>();
            if (player != null)
            {
                player.TakeDamage(damage, cachedKnockbackDir);
            }
        }
    }
}
