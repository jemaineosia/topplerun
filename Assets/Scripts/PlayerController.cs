using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private bool isHurt;

    [Header("Sound Settings")]
    public AudioClip jumpClip; // assign in Inspector
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isHurt)
        {
            // Prevent control while hurt
            animator.SetBool("isWalking", false);
            animator.SetBool("isJumping", false);
            return;
        }

        // Horizontal movement
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on direction
        if (move != 0)
            spriteRenderer.flipX = move < 0;

        // Animation: Walking
        animator.SetBool("isWalking", Mathf.Abs(move) > 0.1f);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Animation: Jumping
        animator.SetBool("isJumping", !isGrounded);

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (jumpClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpClip);
            }
        }
    }

    // Call this method when the player gets hurt
    public void Hurt()
    {
        StartCoroutine(BlinkRed());
        isHurt = true;
        animator.SetBool("isHurt", true);
        rb.linearVelocity = Vector2.zero;
        // Optionally push the player back or trigger invincibility
    }

    private IEnumerator BlinkRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    // Call this from an Animation Event at the end of the "Hurt" animation
    public void RecoverFromHurt()
    {
        isHurt = false;
        animator.SetBool("isHurt", false);
    }

    // Optional: Draw ground check gizmo in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}