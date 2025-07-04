using Assets.src.Scripts.Enemies.BossSlime;
using UnityEngine;
/// <summary>
/// Clase base para los ataques de los jefes.
/// /// Define la estructura básica para los ataques de los jefes,
/// incluyendo la referencia al enemigo jefe y al animador.
/// Debe ser extendida por clases específicas de ataques.
/// </summary>
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
