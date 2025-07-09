using UnityEngine;
/// <summary>
/// Clase base para todos los enemigos del juego.
/// Esta clase define las propiedades y comportamientos comunes de los enemigos,
/// incluyendo salud, física, control de comportamiento, animaciones y gestión de escena.
/// Debe ser extendida por clases específicas de enemigos para implementar comportamientos particulares.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100f;

    //se separan los componentes para cumplir con el principio de responsabilidad única
    //y facilitar la inyección de dependencias
    
    protected EnemyHealth health;
    protected EnemyPhysics physics;
    protected EnemyBehaviorController behaviorController;
    protected EnemyAnimatorController animatorController;
    protected EnemySceneManager sceneManager;
    public Rigidbody2D Rb => physics.rb;
    protected bool isStunned = false;
    public bool IsStunned => isStunned;
    public float AttackRange => behaviorController.attackRange;
    public float VisionRange => behaviorController.visionRange;
    public float SpeedRun => behaviorController.speedRun;
    public GameObject Target => behaviorController.target;
    public EnemyBehaviorController BehaviorController => behaviorController;
    public EnemyAnimatorController AnimatorController => animatorController;
    public virtual void InjectAnimator(IEnemyAnimator animator)
    {
        animatorController?.InjectAnimator(animator);
    }

    public virtual void Initialize()
    {
        health = GetComponent<EnemyHealth>();
        physics = GetComponent<EnemyPhysics>();
        behaviorController = GetComponent<EnemyBehaviorController>();
        animatorController = GetComponent<EnemyAnimatorController>();
        sceneManager = GetComponent<EnemySceneManager>();

        if (health == null || physics == null || behaviorController == null || animatorController == null)
        {
            Debug.LogError("Required components missing on " + gameObject.name);
            return;
        }
        // Se inicializan los componentes
        health.Initialize(maxHealth);
        physics.Initialize();
        behaviorController.Initialize(this);
        animatorController.Initialize();
        behaviorController.target = GameObject.FindWithTag("Player");
    }

    protected virtual void Update()
    {
        if (!health.IsHurtActive)
        {
            UpdateBehavior();
        }
    }


    public void SetBehavior(IEnemyBehavior behavior)
    {
        behaviorController.SetBehavior(behavior);
    }

    public virtual void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce = 5f)
    {
        health.TakeDamage(damage, knockbackDirection, knockbackForce, this is IShieldEnemy, physics, animatorController);
    }

    internal virtual void Die()
    {
        health.Die(this is BossEnemy, sceneManager, animatorController, physics);
    }

    public virtual void UpdateBehavior()
    {
        behaviorController.UpdateBehavior();
    }
}