using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float flySpeed = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Flight")]
    public float maxFlyTime = 3f;
    private float flyTimer;
    private bool isFlying = false;

    private Rigidbody2D rb;
    public bool isGrounded;
    private bool facingRight = true;
    private float verticalVelocity;  // ← we control Y ourselves now

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        flyTimer = maxFlyTime;
    }

    void Update()
    {
        CheckGrounded();
        HandleJumpAndFlight();  // ← flight/jump first, sets verticalVelocity
        HandleMovement();       // ← movement last, applies final velocity
        HandleFlip();
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
      
        if (isGrounded)
        {
            isFlying = false;
            flyTimer = maxFlyTime;
        }
    }

    void HandleJumpAndFlight()
    {
        // JUMP — only from ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = jumpForce;
            isFlying = false;
            return;
        }

        // TOGGLE FLIGHT — only when airborne
        if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && !isFlying)
        {
            isFlying = true;
            flyTimer = maxFlyTime;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isFlying)
        {
            isFlying = false;
        }

        // APPLY FLIGHT
        if (isFlying && flyTimer > 0)
        {
            flyTimer -= Time.deltaTime;
            rb.gravityScale = 0f;

            float flyInput = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                flyInput = 1f;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                flyInput = -1f;

            verticalVelocity = flyInput * flySpeed;

            if (flyTimer <= 0)
                isFlying = false;
        }
        else if (!isFlying)
        {
            // Let physics handle gravity normally
            rb.gravityScale = 2.5f;
            verticalVelocity = rb.linearVelocity.y; // preserve physics Y
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Apply both X and Y together in ONE velocity assignment
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, verticalVelocity);
    }

    void HandleFlip()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}