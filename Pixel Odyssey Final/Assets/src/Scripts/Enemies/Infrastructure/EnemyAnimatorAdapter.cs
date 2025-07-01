using UnityEngine;
/// <summary>
/// Clase adaptadora para manejar las animaciones de enemigos.
/// Implementa la interfaz IEnemyAnimator y IShieldEnemyAnimator para reproducir animaciones de caminar, correr, atacar, recibir daño, morir y escudo.
/// Esta clase debe ser asignada a un GameObject que tenga un componente Animator.
/// </summary>
public class EnemyAnimatorAdapter : MonoBehaviour, IEnemyAnimator, IShieldEnemyAnimator
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void PlayWalk(bool value) => animator.SetBool("walk", value);
    public void PlayRun(bool value) => animator.SetBool("run", value);
    public void PlayAttack(bool value) => animator.SetBool("attack", value);
    public void PlayHurt(bool value) => animator.SetBool("hurt", value);
    public void PlayDie() => animator.SetTrigger("die");
    public bool IsHurt() => animator.GetBool("hurt");
    public Animator Animator => animator;
    public void PlayShield(bool value) => animator.SetBool("shield", value);

}