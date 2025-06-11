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
    public void PlayHurt(bool value) => animator.SetBool("hurt", value);
    public void PlayDie() => animator.SetTrigger("die");
    public bool IsHurt() => animator.GetBool("hurt");
    public Animator Animator => animator;
}