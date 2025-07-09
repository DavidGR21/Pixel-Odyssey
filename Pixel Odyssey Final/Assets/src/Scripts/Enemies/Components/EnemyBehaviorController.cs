using UnityEngine;
/// <summary>
/// Clase encargada de controlar el comportamiento de los enemigos.
/// Esta clase utiliza un IEnemyBehavior para definir el comportamiento actual del enemigo.
/// Permite establecer y actualizar el comportamiento, así como acceder a propiedades comunes como velocidad, rango de visión y ataque.
/// Debe ser extendida por clases específicas de enemigos para implementar comportamientos particulares.
/// </summary>
public class EnemyBehaviorController : MonoBehaviour
{
    private IEnemyBehavior currentBehavior;
    public float speedWalk;
    public float speedRun;
    public float visionRange;
    public float attackRange;
    public int direction;
    public int rutine;
    public float chronometer;
    public GameObject target;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.1f;

    public void Initialize(Enemy enemy)
    {

    }

    public void SetBehavior(IEnemyBehavior behavior)
    {
        currentBehavior = behavior;
    }
    public IEnemyBehavior GetCurrentBehavior()
    {
        return currentBehavior;
    }
    public void UpdateBehavior()
    {
        currentBehavior?.Execute(GetComponent<Enemy>());
    }
}