using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Core player controller for ToppleRun with basic movement and jump mechanics.
/// Handles responsive horizontal movement and jump physics with ground detection.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayerMask = 1;
    
    // Components
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    
    // Input
    private Vector2 moveInput;
    private bool jumpInput;
    
    // State
    private bool isGrounded;
    
    private void Awake()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        // Configure physics settings
        rb.freezeRotation = true; // Prevent rotation on Z-axis
        
        // Create ground check point if not assigned
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -boxCollider.bounds.extents.y, 0);
            groundCheck = groundCheckObj.transform;
        }
    }
    
    private void Update()
    {
        CheckGroundStatus();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }
    
    /// <summary>
    /// Handles horizontal movement based on input
    /// </summary>
    private void HandleMovement()
    {
        // Apply horizontal movement while preserving vertical velocity
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }
    
    /// <summary>
    /// Handles jump mechanics with ground checking
    /// </summary>
    private void HandleJump()
    {
        // Only jump if grounded and jump input is pressed
        if (jumpInput && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        
        // Reset jump input after processing (prevents continuous jumping)
        if (jumpInput)
        {
            jumpInput = false;
        }
    }
    
    /// <summary>
    /// Checks if player is on ground using raycast
    /// </summary>
    private void CheckGroundStatus()
    {
        // Cast a ray downward from the ground check position
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position, 
            Vector2.down, 
            groundCheckDistance, 
            groundLayerMask
        );
        
        isGrounded = hit.collider != null;
    }
    
    #region Input System Callbacks
    
    /// <summary>
    /// Called when move input is received (WASD, Arrow Keys, Gamepad)
    /// </summary>
    /// <param name="context">Input action context</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// Called when jump input is received (Space, Gamepad Button)
    /// </summary>
    /// <param name="context">Input action context</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
    }
    
    #endregion
    
    #region Debug Visualization
    
    private void OnDrawGizmosSelected()
    {
        // Draw ground check ray for debugging
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance);
        }
    }
    
    #endregion
    
    #region Public Properties (for UI/Debugging)
    
    public bool IsGrounded => isGrounded;
    public Vector2 Velocity => rb.velocity;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    
    #endregion
}