using Assets.src.Scripts.Enemies.BossSlime;
using UnityEngine;

public abstract class BossAttackBase : MonoBehaviour, IAttackBehavior
{
    protected BossEnemy boss;
    protected Animator animator;

    public virtual void Start()
    {
        boss = GetComponentInParent<BossEnemy>();
        animator = GetComponentInParent<Animator>();
    }

    public abstract bool CanAttack();
    public abstract void ExecuteAttack();
}
