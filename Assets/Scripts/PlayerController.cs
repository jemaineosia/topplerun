using UnityEngine;

/// <summary>
/// PlayerController handles player movement, jumping, and ground detection for ToppleRun.
/// Supports both keyboard and mobile touch controls.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayerMask = 1;
    
    [Header("Physics")]
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    
    // Components
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    
    // Input tracking
    private float horizontalInput;
    private bool jumpInput;
    private bool isGrounded;
    
    // Mobile touch controls
    private bool isTouchingLeft;
    private bool isTouchingRight;
    private bool isTouchingJump;

    void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        // Configure rigidbody
        rb.gravityScale = gravityScale;
        rb.freezeRotation = true; // Prevent spinning
        
        // Auto-create ground check if not assigned
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -boxCollider.bounds.extents.y - 0.1f, 0);
            groundCheck = groundCheckObj.transform;
        }
        
        // Warn if no ground layer set
        if (groundLayerMask == 0)
        {
            Debug.LogWarning("PlayerController: No ground layer mask set. Ground detection may not work properly.");
        }
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
        HandlePhysics();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }
    
    /// <summary>
    /// Handles both keyboard and touch input
    /// </summary>
    private void HandleInput()
    {
        // Keyboard input
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        
        // Mobile touch input
        HandleTouchInput();
        
        // Combine touch input with keyboard input
        if (isTouchingLeft && !isTouchingRight)
            horizontalInput = -1f;
        else if (isTouchingRight && !isTouchingLeft)
            horizontalInput = 1f;
        else if (!isTouchingLeft && !isTouchingRight && Input.touchCount == 0)
            horizontalInput = Input.GetAxis("Horizontal"); // Fall back to keyboard
            
        if (isTouchingJump)
            jumpInput = true;
    }
    
    /// <summary>
    /// Handles mobile touch controls
    /// Touch left side of screen to move left, right side to move right
    /// Touch upper half of screen to jump
    /// </summary>
    private void HandleTouchInput()
    {
        isTouchingLeft = false;
        isTouchingRight = false;
        isTouchingJump = false;
        
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPos = touch.position;
            
            // Convert touch position to screen percentage
            float screenWidthPercent = touchPos.x / Screen.width;
            float screenHeightPercent = touchPos.y / Screen.height;
            
            // Check for jump (upper half of screen)
            if (screenHeightPercent > 0.5f && touch.phase == TouchPhase.Began)
            {
                isTouchingJump = true;
            }
            
            // Check for movement (lower half of screen)
            if (screenHeightPercent <= 0.5f && (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved))
            {
                if (screenWidthPercent < 0.5f)
                    isTouchingLeft = true;
                else
                    isTouchingRight = true;
            }
        }
    }
    
    /// <summary>
    /// Checks if player is grounded using Physics2D.OverlapCircle
    /// </summary>
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }
    
    /// <summary>
    /// Handles horizontal movement
    /// </summary>
    private void HandleMovement()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = horizontalInput * moveSpeed;
        rb.velocity = velocity;
    }
    
    /// <summary>
    /// Handles jumping with responsive controls
    /// </summary>
    private void HandleJump()
    {
        if (jumpInput && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        
        // Reset jump input
        jumpInput = false;
    }
    
    /// <summary>
    /// Enhanced physics for better feel
    /// </summary>
    private void HandlePhysics()
    {
        // Apply extra gravity when falling
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Apply less gravity when jumping and not holding jump
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Draws gizmos for ground check visualization in Scene view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    
    // Public properties for other scripts to access
    public bool IsGrounded => isGrounded;
    public float HorizontalInput => horizontalInput;
    public Vector2 Velocity => rb.velocity;
}