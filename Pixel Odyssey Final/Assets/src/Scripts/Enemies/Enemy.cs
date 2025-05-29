using UnityEngine;

public class enemy : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private float damageEnemy = 5f;
    private Animator animator;
    private Rigidbody2D rb;
    private HealthPlayer healthPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthPlayer playerHealth = collision.gameObject.GetComponent<HealthPlayer>();
            if (playerHealth != null)
            {
                Vector2 knockbackDirection = collision.transform.position - transform.position;
                playerHealth.TakeDamage(damageEnemy, knockbackDirection);
            }
        }
    }


    public void takeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 1f)
    {
        life -= damage;

        // Retroceso
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        if (life <= 0)
        {
            Dead();
        }
    }

    // Update is called once per frame
    private void Dead() 
    {
        animator.SetTrigger("Dead");
    }
}
