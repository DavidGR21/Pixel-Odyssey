using UnityEngine;
using System.Collections;

public class EnemyPhysics : MonoBehaviour
{
    private Rigidbody2D _rb;
    public Rigidbody2D rb => _rb;

    public void Initialize()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        }
    }

    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(StopMovementAfterKnockback());
        }
    }

    public void DisablePhysics()
    {
        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Static;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator StopMovementAfterKnockback()
    {
        yield return new WaitForSeconds(0.2f);
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }
}