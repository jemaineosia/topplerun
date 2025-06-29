using UnityEngine;
using System.Collections;

/// <summary>
/// LavaRiser system for ToppleRun - makes lava rise from the bottom with player-dependent acceleration
/// Handles burn state when player touches lava and platform detection for burn cancellation
/// </summary>
public class LavaRiser : MonoBehaviour
{
    [Header("Lava Rising Settings")]
    [SerializeField] private float initialSpeed = 1f;
    [SerializeField] private float accelerationFactor = 0.1f;
    [SerializeField] private float maxSpeed = 10f;
    
    [Header("Burn State Settings")]
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private LayerMask platformLayerMask = 1; // Default layer
    
    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool enableBurnVisuals = true;
    [SerializeField] private Color burnColor = Color.red;
    [SerializeField] private float flashSpeed = 10f;
    
    // Private state
    private float currentSpeed;
    private bool playerIsBurning = false;
    private Coroutine burnCoroutine;
    private Color originalPlayerColor;
    private Coroutine flashCoroutine;
    
    // Events for game over (can be subscribed to by game manager)
    public static System.Action OnGameOver;
    
    void Start()
    {
        currentSpeed = initialSpeed;
        
        // Try to find player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerCollider = player.GetComponent<Collider2D>();
                playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            }
        }
        
        // Store original player color for burn effect
        if (playerSpriteRenderer != null)
        {
            originalPlayerColor = playerSpriteRenderer.color;
        }
        
        // Ensure this object has a trigger collider
        Collider2D lavaCollider = GetComponent<Collider2D>();
        if (lavaCollider == null)
        {
            Debug.LogWarning("LavaRiser: No Collider2D found. Adding BoxCollider2D as trigger.");
            lavaCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        lavaCollider.isTrigger = true;
    }
    
    void Update()
    {
        MoveLavaUp();
        
        // Check if player is on platform to cancel burn
        if (playerIsBurning && IsPlayerOnPlatform())
        {
            CancelBurn();
        }
    }
    
    /// <summary>
    /// Moves the lava upward with speed based on player height
    /// </summary>
    private void MoveLavaUp()
    {
        if (playerTransform != null)
        {
            // Calculate speed based on player height - higher player = faster lava
            float heightMultiplier = Mathf.Max(0, playerTransform.position.y * accelerationFactor);
            currentSpeed = Mathf.Min(initialSpeed + heightMultiplier, maxSpeed);
        }
        else
        {
            currentSpeed = initialSpeed;
        }
        
        // Move lava up
        transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Checks if player is standing on a platform using raycast
    /// </summary>
    private bool IsPlayerOnPlatform()
    {
        if (playerCollider == null) return false;
        
        // Cast a ray downward from player to check for ground
        Vector2 rayOrigin = playerCollider.bounds.center;
        float rayDistance = playerCollider.bounds.extents.y + 0.1f;
        
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, platformLayerMask);
        
        return hit.collider != null;
    }
    
    /// <summary>
    /// Triggered when player enters lava
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == playerTransform && !playerIsBurning)
        {
            StartBurn();
        }
    }
    
    /// <summary>
    /// Triggered when player exits lava
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == playerTransform)
        {
            // Player left lava area - they might be safe if on platform
            // The Update method will check if they're on a platform and cancel burn if so
        }
    }
    
    /// <summary>
    /// Starts the burn countdown
    /// </summary>
    private void StartBurn()
    {
        if (playerIsBurning) return;
        
        playerIsBurning = true;
        
        // Start burn countdown
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }
        burnCoroutine = StartCoroutine(BurnCountdown());
        
        // Start visual effects
        if (enableBurnVisuals && playerSpriteRenderer != null)
        {
            StartBurnVisuals();
        }
        
        Debug.Log("Player is burning! Get to a platform within " + burnDuration + " seconds!");
    }
    
    /// <summary>
    /// Cancels the burn state when player reaches safety
    /// </summary>
    private void CancelBurn()
    {
        if (!playerIsBurning) return;
        
        playerIsBurning = false;
        
        // Stop burn countdown
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }
        
        // Stop visual effects
        StopBurnVisuals();
        
        Debug.Log("Burn cancelled! Player reached safety.");
    }
    
    /// <summary>
    /// Burn countdown coroutine
    /// </summary>
    private IEnumerator BurnCountdown()
    {
        yield return new WaitForSeconds(burnDuration);
        
        // If we reach here, player didn't reach safety in time
        TriggerGameOver();
    }
    
    /// <summary>
    /// Triggers game over
    /// </summary>
    private void TriggerGameOver()
    {
        playerIsBurning = false;
        StopBurnVisuals();
        
        Debug.Log("Game Over! Player burned in lava.");
        
        // Trigger game over event for any listeners
        OnGameOver?.Invoke();
        
        // If no game manager is listening, pause the game
        if (OnGameOver == null || OnGameOver.GetInvocationList().Length == 0)
        {
            Time.timeScale = 0;
            Debug.Log("Game paused. No game manager found to handle game over.");
        }
    }
    
    /// <summary>
    /// Starts visual burn effects
    /// </summary>
    private void StartBurnVisuals()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashPlayer());
    }
    
    /// <summary>
    /// Stops visual burn effects
    /// </summary>
    private void StopBurnVisuals()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        
        // Restore original color
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = originalPlayerColor;
        }
    }
    
    /// <summary>
    /// Flashes player color to indicate burn state
    /// </summary>
    private IEnumerator FlashPlayer()
    {
        while (playerIsBurning)
        {
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.color = burnColor;
                yield return new WaitForSeconds(1f / flashSpeed);
                
                playerSpriteRenderer.color = originalPlayerColor;
                yield return new WaitForSeconds(1f / flashSpeed);
            }
            else
            {
                yield return null;
            }
        }
    }
    
    /// <summary>
    /// Public method to set player reference (useful for runtime setup)
    /// </summary>
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
            playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            
            if (playerSpriteRenderer != null)
            {
                originalPlayerColor = playerSpriteRenderer.color;
            }
        }
    }
    
    /// <summary>
    /// Public method to get current lava speed (useful for UI)
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    /// <summary>
    /// Public method to check if player is currently burning
    /// </summary>
    public bool IsPlayerBurning()
    {
        return playerIsBurning;
    }
}