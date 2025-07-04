// MovementPlayer.cs
using Assets.src.Scripts.Player.States;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    // Uso del Patr�n State:
    // Se implementa el patr�n State mediante la clase PlayerMovementContext y sus estados asociados (por ejemplo, NormalMovementState).
    // Este patr�n permite cambiar din�micamente el comportamiento del jugador dependiendo de su estado actual (normal, saltando, dash, etc.)
    // sin usar m�ltiples condicionales en una sola clase. Esto facilita la extensibilidad, el mantenimiento y el orden del c�digo.
    
    [Header("Movement Settings")]
    [SerializeField] public float velocityOfMovement = 10f;
    [Range(0, 0.3f)][SerializeField] public float motionSmotothing = 0.3f;

    [Header("Jump Settings")]
    [SerializeField] public float jumpForce;
    [SerializeField] public LayerMask isFloor;
    [SerializeField] public Transform floorControler;
    [SerializeField] public Vector3 boxDimension;

    [Header("Dash Settings")]
    [SerializeField] public float dashSpeed;
    [SerializeField] public float dashTime;
    [SerializeField] public float timeIntoDash;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip stepClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip dashClip;

    [HideInInspector] public PlayerAudioHandler audioHandler;


    // Component references
    [HideInInspector] public Rigidbody2D rb2d;
    [HideInInspector] public Animator animator;
    [HideInInspector] public atackPlayer attackScript;

    // State variables
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public bool inFloor;
    [HideInInspector] public bool jump;
    [HideInInspector] public bool lookingRight = true;
    [HideInInspector] public bool canDoDash = true;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public float initialGravity;

    [HideInInspector] public Vector2 velocity = Vector2.zero;
    private PlayerMovementContext movementContext;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackScript = GetComponent<atackPlayer>();
        initialGravity = rb2d.gravityScale;

        // Inicializar el handler con los clips
        audioHandler = new PlayerAudioHandler(stepClip, jumpClip, damageClip, attackClip, dashClip);

        movementContext = new PlayerMovementContext(this);
        movementContext.TransitionTo(new NormalMovementState());
    }


    private void Update() => movementContext.Update();
    private void FixedUpdate() => movementContext.FixedUpdate();

    public void spin()
    {
        lookingRight = !lookingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void BlockPlayerControl(bool block)
    {
        canMove = !block;
        if (attackScript != null)
        {
            attackScript.canMove = !block;
        }

        animator.speed = block ? 0f : 1f;

        if (block)
        {
            rb2d.linearVelocity = Vector2.zero; // Detener completamente
        }

        Debug.Log($"[Jugador] Control {(block ? "bloqueado" : "restablecido")}");
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(floorControler.position, boxDimension);
    }

}