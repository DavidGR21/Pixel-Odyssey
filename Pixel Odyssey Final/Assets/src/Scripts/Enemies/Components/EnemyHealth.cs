using UnityEngine;
/// <summary>
/// Clase encargada de manejar la salud de los enemigos.
/// Esta clase permite inicializar la salud máxima, recibir daño, aplicar retroceso y manejar la muerte del enemigo.
/// También gestiona el estado de "herido" para activar animaciones de daño.
/// Debe ser utilizada por cualquier clase que implemente un enemigo en el juego.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    private float currentHealth;
    private float maxHealth;
    private bool isHurtActive = false;
    public bool IsHurtActive => isHurtActive;
    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce, bool isShielded, EnemyPhysics physics, EnemyAnimatorController animatorController)
    {
        float adjustedDamage = damage;
        if (isShielded && GetComponent<IShieldEnemy>().TakeShieldedDamage(damage, out adjustedDamage))
        {
            return;
        }

        currentHealth -= adjustedDamage;
        if (adjustedDamage > 0)
        {
            isHurtActive = true;
            animatorController.SetHurtAnimation(true);
            physics.ApplyKnockback(knockbackDirection, knockbackForce);
            animatorController.ResetHurtAnimation(() => isHurtActive = false);
        }

        if (currentHealth <= 0)
        {
            GetComponent<Enemy>().Die();
        }
    }

    public void Die(bool isBoss, EnemySceneManager sceneManager, EnemyAnimatorController animatorController, EnemyPhysics physics)
    {
        animatorController.SetDeathAnimation();
        physics.DisablePhysics();
        if (isBoss)
        {
            sceneManager.LoadCreditsScene();
        }
        else
        {
            sceneManager.DestroyAfterDeathAnimation();
        }
    }
}