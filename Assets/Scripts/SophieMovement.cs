using UnityEngine;

public class SophieMovement : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float speed;
    public float jumpPower;
    public float moveDirection;
    private bool facingRight = false;
    private bool isJumping = false;
    private bool hasJumped = false;
    private Animator animator;
    public LayerMask groundLayer;
    private Vector2 boxSize = new Vector2(0.1f, 1f);

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ProcessInputs();
        Animate();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void ProcessInputs()
    {
        moveDirection = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isJumping && !hasJumped)
        {
            isJumping = true;
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpPower);
            hasJumped = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteraction();
            Debug.Log("E key pressed");
        }
    }

    private void Move()
    {
        playerRb.velocity = new Vector2(moveDirection * speed, playerRb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && isJumping) // Check if collided object is on the ground layer and the player is jumping
        {
            hasJumped = false;
            isJumping = false;
        }
    }

    private void Animate()
    {
        if (moveDirection > 0 && !facingRight)
        {
            FlipCharacter();
        }
        else if (moveDirection < 0 && facingRight)
        {
            FlipCharacter();
        }
        animator.SetFloat("horizontalValue", Mathf.Abs(moveDirection));
        animator.SetBool("isJumping", isJumping);
    }

    private void FlipCharacter()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void CheckInteraction()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, 0, Vector2.zero);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D rc in hits)
            {
                if (rc.transform.GetComponent<Interactable>())
                {
                    rc.transform.GetComponent<Interactable>().Interact();
                    return;
                }
            }
        }
    }
}
