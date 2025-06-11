using UnityEngine;

public class EnemyAnimatorAdapter : MonoBehaviour, IEnemyAnimator
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
    public void PlayHurt() => animator.SetTrigger("Hurt");
    public void PlayDie() => animator.SetTrigger("Die");
    public bool IsHurt() => animator.GetBool("hurt");
}