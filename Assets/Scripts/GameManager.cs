using UnityEngine;

/// <summary>
/// GameManager coordinates the core ToppleRun gameplay systems.
/// This is an optional helper script that demonstrates how to use PlayerController, 
/// BlockSpawner, and BlockCleanup together.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BlockSpawner blockSpawner;
    [SerializeField] private Camera gameCamera;
    
    [Header("Game Settings")]
    [SerializeField] private bool autoStartGame = true;
    [SerializeField] private float cameraFollowSpeed = 2f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 3f, -10f);
    
    [Header("Performance Settings")]
    [SerializeField] private int maxActiveBlocks = 50;
    [SerializeField] private float cleanupInterval = 5f;
    
    // Game state
    private bool gameStarted = false;
    private float lastCleanupTime;
    
    void Start()
    {
        // Auto-find components if not assigned
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        
        if (blockSpawner == null)
            blockSpawner = FindObjectOfType<BlockSpawner>();
        
        if (gameCamera == null)
            gameCamera = Camera.main ?? FindObjectOfType<Camera>();
        
        // Validate setup
        ValidateSetup();
        
        // Auto-start if enabled
        if (autoStartGame)
        {
            StartGame();
        }
    }
    
    void Update()
    {
        if (!gameStarted) return;
        
        UpdateCameraFollow();
        UpdateBlockSpawning();
        UpdatePerformanceManagement();
    }
    
    /// <summary>
    /// Validates that all required components are present
    /// </summary>
    private void ValidateSetup()
    {
        if (playerController == null)
        {
            Debug.LogError("GameManager: No PlayerController found! Please assign one or add PlayerController script to a GameObject.");
        }
        
        if (blockSpawner == null)
        {
            Debug.LogWarning("GameManager: No BlockSpawner found. Block spawning will be disabled.");
        }
        
        if (gameCamera == null)
        {
            Debug.LogError("GameManager: No camera found! Camera following will be disabled.");
        }
    }
    
    /// <summary>
    /// Starts the game
    /// </summary>
    public void StartGame()
    {
        if (gameStarted) return;
        
        gameStarted = true;
        
        // Start block spawning
        if (blockSpawner != null)
        {
            blockSpawner.StartSpawning();
        }
        
        Debug.Log("GameManager: Game started!");
    }
    
    /// <summary>
    /// Stops the game
    /// </summary>
    public void StopGame()
    {
        if (!gameStarted) return;
        
        gameStarted = false;
        
        // Stop block spawning
        if (blockSpawner != null)
        {
            blockSpawner.StopSpawning();
        }
        
        Debug.Log("GameManager: Game stopped!");
    }
    
    /// <summary>
    /// Updates camera to follow player smoothly
    /// </summary>
    private void UpdateCameraFollow()
    {
        if (gameCamera == null || playerController == null) return;
        
        Vector3 targetPosition = playerController.transform.position + cameraOffset;
        gameCamera.transform.position = Vector3.Lerp(
            gameCamera.transform.position, 
            targetPosition, 
            cameraFollowSpeed * Time.deltaTime
        );
    }
    
    /// <summary>
    /// Updates block spawning based on player position
    /// </summary>
    private void UpdateBlockSpawning()
    {
        if (blockSpawner == null || playerController == null) return;
        
        // Update spawn height to stay above player
        blockSpawner.UpdateSpawnHeight(10f);
    }
    
    /// <summary>
    /// Manages performance by cleaning up excess blocks
    /// </summary>
    private void UpdatePerformanceManagement()
    {
        // Periodic cleanup check
        if (Time.time - lastCleanupTime > cleanupInterval)
        {
            lastCleanupTime = Time.time;
            
            int activeBlocks = BlockCleanup.GetActiveBlockCount();
            
            if (activeBlocks > maxActiveBlocks)
            {
                Debug.Log($"GameManager: Too many blocks ({activeBlocks}), performing cleanup...");
                
                // Clean up blocks that are old or far from camera
                BlockCleanup.CleanupAllBlocks(maxAge: 30f, maxDistance: 25f);
            }
        }
    }
    
    /// <summary>
    /// Restarts the game by cleaning up all blocks and restarting spawning
    /// </summary>
    public void RestartGame()
    {
        // Clean up all existing blocks
        BlockCleanup.CleanupAllBlocks(maxAge: 0f);
        
        // Reset player position (optional - implement based on your needs)
        if (playerController != null)
        {
            // You might want to reset player to a starting position here
            // playerController.transform.position = Vector3.zero;
        }
        
        // Restart the game
        StopGame();
        StartGame();
        
        Debug.Log("GameManager: Game restarted!");
    }
    
    /// <summary>
    /// Emergency cleanup - removes all blocks immediately
    /// </summary>
    public void EmergencyCleanup()
    {
        BlockCleanup.CleanupAllBlocks(maxAge: 0f);
        Debug.Log("GameManager: Emergency cleanup performed!");
    }
    
    // Public properties for accessing game state
    public bool IsGameActive => gameStarted;
    public PlayerController Player => playerController;
    public BlockSpawner Spawner => blockSpawner;
    
    // UI/Debug methods that could be called from buttons or other scripts
    [ContextMenu("Start Game")]
    public void StartGameFromMenu() => StartGame();
    
    [ContextMenu("Stop Game")]
    public void StopGameFromMenu() => StopGame();
    
    [ContextMenu("Restart Game")]
    public void RestartGameFromMenu() => RestartGame();
    
    [ContextMenu("Emergency Cleanup")]
    public void EmergencyCleanupFromMenu() => EmergencyCleanup();
}