using UnityEngine;

/// <summary>
/// BlockCleanup handles automatic cleanup of blocks that fall too far below the camera
/// or have existed for too long. Prevents memory issues and maintains performance.
/// </summary>
public class BlockCleanup : MonoBehaviour
{
    [Header("Cleanup Settings")]
    [SerializeField] private float distanceBelowCamera = 20f;
    [SerializeField] private float maxLifetime = 60f; // Maximum time before cleanup (in seconds)
    [SerializeField] private bool enableLifetimeCleanup = true;
    [SerializeField] private bool enableDistanceCleanup = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    // Runtime variables
    private Camera mainCamera;
    private float spawnTime;
    private bool isMarkedForDestroy = false;
    
    void Start()
    {
        // Find the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        if (mainCamera == null && enableDistanceCleanup)
        {
            Debug.LogWarning($"BlockCleanup ({gameObject.name}): No camera found! Distance-based cleanup disabled.");
            enableDistanceCleanup = false;
        }
        
        // Record spawn time for lifetime cleanup
        spawnTime = Time.time;
        
        if (showDebugInfo)
        {
            Debug.Log($"BlockCleanup: {gameObject.name} spawned at {spawnTime:F1}s");
        }
    }
    
    void Update()
    {
        if (isMarkedForDestroy) return;
        
        // Check for distance-based cleanup
        if (enableDistanceCleanup && mainCamera != null)
        {
            CheckDistanceCleanup();
        }
        
        // Check for lifetime-based cleanup
        if (enableLifetimeCleanup)
        {
            CheckLifetimeCleanup();
        }
    }
    
    /// <summary>
    /// Checks if the block is too far below the camera and should be cleaned up
    /// </summary>
    private void CheckDistanceCleanup()
    {
        float cameraY = mainCamera.transform.position.y;
        float blockY = transform.position.y;
        float distance = cameraY - blockY;
        
        if (distance > distanceBelowCamera)
        {
            if (showDebugInfo)
            {
                Debug.Log($"BlockCleanup: {gameObject.name} cleaned up by distance. " +
                         $"Camera Y: {cameraY:F1}, Block Y: {blockY:F1}, Distance: {distance:F1}");
            }
            
            CleanupBlock("distance");
        }
    }
    
    /// <summary>
    /// Checks if the block has exceeded its maximum lifetime and should be cleaned up
    /// </summary>
    private void CheckLifetimeCleanup()
    {
        float currentTime = Time.time;
        float lifetime = currentTime - spawnTime;
        
        if (lifetime > maxLifetime)
        {
            if (showDebugInfo)
            {
                Debug.Log($"BlockCleanup: {gameObject.name} cleaned up by lifetime. " +
                         $"Lifetime: {lifetime:F1}s, Max: {maxLifetime:F1}s");
            }
            
            CleanupBlock("lifetime");
        }
    }
    
    /// <summary>
    /// Cleanly destroys the block with optional reason logging
    /// </summary>
    private void CleanupBlock(string reason)
    {
        if (isMarkedForDestroy) return;
        
        isMarkedForDestroy = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"BlockCleanup: Destroying {gameObject.name} (reason: {reason})");
        }
        
        // Optional: Add cleanup effects here (particles, sound, etc.)
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Forces immediate cleanup of this block
    /// </summary>
    public void ForceCleanup()
    {
        CleanupBlock("forced");
    }
    
    /// <summary>
    /// Updates the distance threshold for cleanup (useful for dynamic adjustment)
    /// </summary>
    public void SetDistanceThreshold(float newDistance)
    {
        distanceBelowCamera = newDistance;
    }
    
    /// <summary>
    /// Updates the lifetime threshold for cleanup
    /// </summary>
    public void SetLifetimeThreshold(float newLifetime)
    {
        maxLifetime = newLifetime;
    }
    
    /// <summary>
    /// Gets the current age of this block
    /// </summary>
    public float GetAge()
    {
        return Time.time - spawnTime;
    }
    
    /// <summary>
    /// Gets the distance below the camera
    /// </summary>
    public float GetDistanceBelowCamera()
    {
        if (mainCamera == null) return 0f;
        
        return mainCamera.transform.position.y - transform.position.y;
    }
    
    /// <summary>
    /// Checks if this block is currently marked for destruction
    /// </summary>
    public bool IsMarkedForDestroy => isMarkedForDestroy;
    
    /// <summary>
    /// Static method to cleanup all blocks that match certain criteria
    /// </summary>
    public static void CleanupAllBlocks(float maxAge = -1f, float maxDistance = -1f)
    {
        BlockCleanup[] allCleanupComponents = FindObjectsOfType<BlockCleanup>();
        int cleanedCount = 0;
        
        foreach (BlockCleanup cleanup in allCleanupComponents)
        {
            bool shouldCleanup = false;
            string reason = "";
            
            if (maxAge > 0 && cleanup.GetAge() > maxAge)
            {
                shouldCleanup = true;
                reason = "batch_age";
            }
            else if (maxDistance > 0 && cleanup.GetDistanceBelowCamera() > maxDistance)
            {
                shouldCleanup = true;
                reason = "batch_distance";
            }
            
            if (shouldCleanup)
            {
                cleanup.CleanupBlock(reason);
                cleanedCount++;
            }
        }
        
        if (cleanedCount > 0)
        {
            Debug.Log($"BlockCleanup: Batch cleaned {cleanedCount} blocks");
        }
    }
    
    /// <summary>
    /// Gets count of active blocks with cleanup components
    /// </summary>
    public static int GetActiveBlockCount()
    {
        return FindObjectsOfType<BlockCleanup>().Length;
    }
    
    /// <summary>
    /// Draws debug information in Scene view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showDebugInfo || mainCamera == null) return;
        
        // Draw cleanup threshold line
        Gizmos.color = Color.red;
        float cleanupY = mainCamera.transform.position.y - distanceBelowCamera;
        float leftX = transform.position.x - 2f;
        float rightX = transform.position.x + 2f;
        
        Gizmos.DrawLine(new Vector3(leftX, cleanupY, 0), new Vector3(rightX, cleanupY, 0));
        
        // Draw current distance indicator
        Gizmos.color = Color.yellow;
        Vector3 blockPos = transform.position;
        Vector3 cameraProjection = new Vector3(blockPos.x, mainCamera.transform.position.y, 0);
        Gizmos.DrawLine(blockPos, cameraProjection);
        
        // Show current values
        float distance = GetDistanceBelowCamera();
        float age = GetAge();
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up, 
            $"Age: {age:F1}s\nDistance: {distance:F1}");
        #endif
    }
}