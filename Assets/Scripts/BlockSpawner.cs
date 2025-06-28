using UnityEngine;
using System.Collections;

/// <summary>
/// BlockSpawner handles procedural block generation for ToppleRun.
/// Creates blocks with different sizes, physics properties, and spawning patterns.
/// </summary>
public class BlockSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private Vector2 spawnRangeX = new Vector2(-8f, 8f);
    
    [Header("Block Variations")]
    [SerializeField] private BlockSize[] blockSizes = new BlockSize[]
    {
        new BlockSize("Square", new Vector2(1f, 1f), 1f),
        new BlockSize("Wide", new Vector2(2f, 1f), 1.5f),
        new BlockSize("Tall", new Vector2(1f, 2f), 1.5f),
        new BlockSize("Extra Wide", new Vector2(3f, 1f), 2f)
    };
    
    [Header("Physics Settings")]
    [SerializeField] private PhysicsMaterial2D blockPhysicsMaterial;
    [SerializeField] private Vector2 randomVelocityRange = new Vector2(-2f, 2f);
    [SerializeField] private Vector2 randomRotationRange = new Vector2(-45f, 45f);
    
    [Header("Auto-Spawning")]
    [SerializeField] private bool autoSpawn = true;
    [SerializeField] private float startDelay = 1f;
    
    // Runtime variables
    private Camera mainCamera;
    private bool isSpawning = false;
    
    /// <summary>
    /// Defines different block size variations
    /// </summary>
    [System.Serializable]
    public struct BlockSize
    {
        public string name;
        public Vector2 size;
        public float mass;
        
        public BlockSize(string name, Vector2 size, float mass)
        {
            this.name = name;
            this.size = size;
            this.mass = mass;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
            if (mainCamera == null)
            {
                Debug.LogError("BlockSpawner: No camera found in scene!");
                return;
            }
        }
        
        ValidateSetup();
        
        if (autoSpawn)
        {
            StartSpawning();
        }
    }
    
    /// <summary>
    /// Validates the spawner setup and provides warnings for missing components
    /// </summary>
    private void ValidateSetup()
    {
        if (blockPrefab == null)
        {
            Debug.LogError("BlockSpawner: No block prefab assigned! Please assign a prefab with Rigidbody2D and Collider2D.");
            return;
        }
        
        // Check if the prefab has required components
        Rigidbody2D prefabRb = blockPrefab.GetComponent<Rigidbody2D>();
        Collider2D prefabCollider = blockPrefab.GetComponent<Collider2D>();
        
        if (prefabRb == null)
        {
            Debug.LogWarning("BlockSpawner: Block prefab is missing Rigidbody2D component. Physics may not work correctly.");
        }
        
        if (prefabCollider == null)
        {
            Debug.LogWarning("BlockSpawner: Block prefab is missing Collider2D component. Collisions may not work correctly.");
        }
        
        if (blockSizes.Length == 0)
        {
            Debug.LogWarning("BlockSpawner: No block sizes defined. Using default square size.");
        }
        
        if (spawnRangeX.x >= spawnRangeX.y)
        {
            Debug.LogWarning("BlockSpawner: Invalid spawn range X. Min should be less than Max.");
        }
    }
    
    /// <summary>
    /// Starts the automatic block spawning coroutine
    /// </summary>
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnBlocksCoroutine());
        }
    }
    
    /// <summary>
    /// Stops the automatic block spawning
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
    
    /// <summary>
    /// Coroutine that handles automatic block spawning
    /// </summary>
    private IEnumerator SpawnBlocksCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        while (isSpawning)
        {
            SpawnBlock();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    /// <summary>
    /// Spawns a single block with random properties
    /// </summary>
    public GameObject SpawnBlock()
    {
        if (blockPrefab == null)
        {
            Debug.LogError("BlockSpawner: Cannot spawn block - no prefab assigned!");
            return null;
        }
        
        // Calculate spawn position
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0f);
        
        // Get random block size
        BlockSize selectedSize = GetRandomBlockSize();
        
        // Instantiate the block
        GameObject newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        
        // Configure the block
        ConfigureBlock(newBlock, selectedSize);
        
        return newBlock;
    }
    
    /// <summary>
    /// Gets a random block size from the available variations
    /// </summary>
    private BlockSize GetRandomBlockSize()
    {
        if (blockSizes.Length == 0)
        {
            return new BlockSize("Default", Vector2.one, 1f);
        }
        
        int randomIndex = Random.Range(0, blockSizes.Length);
        return blockSizes[randomIndex];
    }
    
    /// <summary>
    /// Configures a spawned block with size, physics, and random properties
    /// </summary>
    private void ConfigureBlock(GameObject block, BlockSize blockSize)
    {
        // Set up transform scale
        block.transform.localScale = new Vector3(blockSize.size.x, blockSize.size.y, 1f);
        
        // Configure Rigidbody2D
        Rigidbody2D rb = block.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = blockSize.mass;
            
            // Apply random velocity
            float randomVelX = Random.Range(randomVelocityRange.x, randomVelocityRange.y);
            rb.velocity = new Vector2(randomVelX, 0f);
            
            // Apply random rotation
            float randomRotation = Random.Range(randomRotationRange.x, randomRotationRange.y);
            rb.angularVelocity = randomRotation;
            
            // Apply physics material if available
            if (blockPhysicsMaterial != null)
            {
                Collider2D collider = block.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.sharedMaterial = blockPhysicsMaterial;
                }
            }
        }
        
        // Add BlockCleanup component if not present
        if (block.GetComponent<BlockCleanup>() == null)
        {
            block.AddComponent<BlockCleanup>();
        }
        
        // Add a unique name for debugging
        block.name = $"Block_{blockSize.name}_{Time.time:F1}";
    }
    
    /// <summary>
    /// Manually spawns a block with a specific size
    /// </summary>
    public GameObject SpawnBlockWithSize(string sizeName)
    {
        BlockSize? targetSize = null;
        
        foreach (BlockSize size in blockSizes)
        {
            if (size.name.Equals(sizeName, System.StringComparison.OrdinalIgnoreCase))
            {
                targetSize = size;
                break;
            }
        }
        
        if (!targetSize.HasValue)
        {
            Debug.LogWarning($"BlockSpawner: Size '{sizeName}' not found. Using random size instead.");
            return SpawnBlock();
        }
        
        // Calculate spawn position
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0f);
        
        // Instantiate and configure the block
        GameObject newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        ConfigureBlock(newBlock, targetSize.Value);
        
        return newBlock;
    }
    
    /// <summary>
    /// Updates spawn height based on camera position (useful for following player)
    /// </summary>
    public void UpdateSpawnHeight(float offset = 10f)
    {
        if (mainCamera != null)
        {
            spawnHeight = mainCamera.transform.position.y + offset;
        }
    }
    
    /// <summary>
    /// Draws gizmos to visualize spawn area in Scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        // Draw spawn area
        Gizmos.color = Color.yellow;
        Vector3 leftPoint = new Vector3(spawnRangeX.x, spawnHeight, 0f);
        Vector3 rightPoint = new Vector3(spawnRangeX.y, spawnHeight, 0f);
        
        // Draw spawn line
        Gizmos.DrawLine(leftPoint, rightPoint);
        
        // Draw spawn range indicators
        Gizmos.DrawWireCube(leftPoint, Vector3.one * 0.5f);
        Gizmos.DrawWireCube(rightPoint, Vector3.one * 0.5f);
        
        // Draw spawn area box
        Vector3 center = new Vector3((spawnRangeX.x + spawnRangeX.y) * 0.5f, spawnHeight, 0f);
        Vector3 size = new Vector3(spawnRangeX.y - spawnRangeX.x, 0.5f, 1f);
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(center, size);
    }
    
    // Public properties for accessing spawner state
    public bool IsSpawning => isSpawning;
    public float SpawnInterval => spawnInterval;
    public BlockSize[] AvailableBlockSizes => blockSizes;
}