using System.Collections;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private float horizontalMovement = 0f;
    [SerializeField] private float velocityOfMovement;
    [Range(0, 0.3f)][SerializeField] private float motionSmotothing = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private bool lookingRight = true;

    // JUMP
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask isFloor;
    [SerializeField] private Transform floorControler;
    [SerializeField] private Vector3 boxDimension;
    [SerializeField] public bool inFloor;
    private bool jump = false;

    //DASH
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float timeIntoDash;
    private float initialGravity;
    private bool canDoDash = true;
    private bool canMove = true;



    // ANIMATION
    private Animator animator;

    // REFERENCIA A atackPlayer
    private atackPlayer attackScript;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackScript = GetComponent<atackPlayer>(); // <- Referencia al script de ataque
        initialGravity = rb2d.gravityScale;
    }

    private void Update()
    {
        // Solo se mueve si no est� atacando
        if (attackScript == null || attackScript.canMove)
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal") * velocityOfMovement;
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            }
        }
        else
        {
            horizontalMovement = 0f;
        }

        animator.SetFloat("Horizontal", Mathf.Abs(horizontalMovement));
        if (Input.GetKeyDown(KeyCode.C) && canDoDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        inFloor = Physics2D.OverlapBox(floorControler.position, boxDimension, 0f, isFloor);
        animator.SetBool("inFloor", inFloor);
        if (canMove) 
        { 
            Move(horizontalMovement * Time.fixedDeltaTime, jump);
        }
        jump = false;
    }

    private void Move(float move, bool jump)
    {
        Vector3 objectiveVelocity = new Vector2(move, rb2d.linearVelocity.y);
        rb2d.linearVelocity = Vector3.SmoothDamp(rb2d.linearVelocity, objectiveVelocity, ref velocity, motionSmotothing);

        if (move > 0 && !lookingRight)
        {
            spin();
        }
        else if (move < 0 && lookingRight)
        {
            spin();
        }

        if (inFloor && jump)
        {
            inFloor = false;
            rb2d.AddForce(new Vector2(0f, jumpForce));
        }
    }

    private IEnumerator Dash()
    { 
        canMove = false;
        canDoDash = false;
        rb2d.gravityScale = 0;
        rb2d.linearVelocity = new Vector2(dashSpeed * transform.localScale.x,0);
        animator.SetTrigger("Dash");

        yield return new WaitForSeconds(dashTime);

        canMove = true;
        rb2d.gravityScale = initialGravity;
        yield return new WaitForSeconds(timeIntoDash);
        canDoDash = true;

    }

    private void spin()
    {
        lookingRight = !lookingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(floorControler.position, boxDimension);
    }
}
