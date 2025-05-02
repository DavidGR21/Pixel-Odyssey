using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private float horizontalMovement = 0f;
    [SerializeField] private float velocityOfMovement;
    [Range(0,0.3f)] [SerializeField] private float motionSmotothing = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private bool lookingRight = true;

    //JUMP
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask isFloor;
    [SerializeField] private Transform floorControler;
    [SerializeField] private Vector3 boxDimension;
    [SerializeField] private bool inFloor;
    private bool jump = false;

    //ANIMATION
    private Animator animator;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal") * velocityOfMovement;
        animator.SetFloat("Horizontal", Mathf.Abs(horizontalMovement));
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        inFloor = Physics2D.OverlapBox(floorControler.position, boxDimension, 0f, isFloor);
        animator.SetBool("inFloor", inFloor);
        //Move Player
        Move(horizontalMovement * Time.fixedDeltaTime, jump);
        jump = false;

    }

    private void Move(float move, bool jump)
    {
        Vector3 objectiveVelocity = new Vector2(move,rb2d.linearVelocity.y);
        rb2d.linearVelocity = Vector3.SmoothDamp(rb2d.linearVelocity, objectiveVelocity, ref velocity, motionSmotothing);
        if (move > 0 && !lookingRight) {
            //Girar
            spin();
        }else if (move < 0 && lookingRight)
        {
            //Girar
            spin();
        }

        if (inFloor && jump)
        {
            inFloor = false;
            rb2d.AddForce(new Vector2(0f,jumpForce));
        }
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
