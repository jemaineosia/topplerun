using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Block Spawning")]
    public GameObject blockPrefab;
    public float spawnInterval = 2f;
    public float spawnHeight = 10f;
    public Sprite[] blockSprites;

    [Header("Spawn Area")]
    public float spawnRangeX = 8f; // How wide the spawn area is
    public Vector2 spawnOffset = Vector2.zero; // Offset from spawner position

    [Header("Block Variations")]
    public Vector2[] blockSizes = {
        new Vector2(2f, 1f),    // Wide block
        new Vector2(1f, 1f),    // Square block
        new Vector2(1f, 2f),    // Tall block
        new Vector2(3f, 1f)     // Extra wide block
    };

    [Header("Physics Settings")]
    public float blockMass = 1f;
    public PhysicsMaterial2D blockPhysicsMaterial;

    [Header("Lava Follow")]
    public Transform player; // Assign in Inspector
    public float spawnHeightOffset = 10f; // How far above lava to spawn

    private float nextSpawnTime;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        nextSpawnTime = Time.time + spawnInterval;

        // If no block prefab assigned, create a warning
        if (blockPrefab == null)
        {
            Debug.LogWarning("BlockSpawner: No block prefab assigned! Please assign a block prefab.");
        }
    }

    void Update()
    {
        // Move the spawner to follow the lava
        if (player != null)
        {
            transform.position = new Vector3(transform.position.x, player.position.y + spawnHeightOffset, transform.position.z);
        }

        // Check if it's time to spawn new blocks
        if (Time.time >= nextSpawnTime && blockPrefab != null)
        {
            SpawnBlocks(2); // Spawn two blocks per drop
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnBlocks(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBlock();
        }
    }

    void SpawnBlock()
    {
        // Calculate spawn position
        float randomX = Random.Range(-spawnRangeX / 2f, spawnRangeX / 2f);
        Vector3 spawnPosition = new Vector3(
            transform.position.x + randomX + spawnOffset.x,
            transform.position.y + spawnHeight + spawnOffset.y,
            0f
        );

        // Instantiate the block
        GameObject newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        newBlock.layer = LayerMask.NameToLayer("Ground");

        // Assign random sprite
        if (blockSprites != null && blockSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, blockSprites.Length);
            SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = blockSprites[randomIndex];
                newBlock.transform.localScale = Vector3.one * 0.5f; // Adjust as needed

                BoxCollider2D collider = newBlock.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                    collider.size = sr.sprite.bounds.size;
                    collider.offset = sr.sprite.bounds.center;
                }
            }
        }

        // Apply random size variation
        //ApplyBlockVariation(newBlock);

        // Setup physics
        SetupBlockPhysics(newBlock);

        // Optional: Add slight random rotation for more natural falling
        float randomRotation = Random.Range(-5f, 5f);
        newBlock.transform.rotation = Quaternion.Euler(0, 0, randomRotation);

        // Optional: Add slight horizontal velocity for variety
        Rigidbody2D blockRb = newBlock.GetComponent<Rigidbody2D>();
        if (blockRb != null)
        {
            float randomVelocity = Random.Range(-1f, 1f);
            blockRb.linearVelocity = new Vector2(randomVelocity, 0);
        }
    }

    void ApplyBlockVariation(GameObject block)
    {
        if (blockSizes.Length > 0)
        {
            // Choose random block size
            Vector2 chosenSize = blockSizes[Random.Range(0, blockSizes.Length)];

            // Apply size to transform
            block.transform.localScale = new Vector3(chosenSize.x, chosenSize.y, 1f);

            // Adjust collider size if present
            BoxCollider2D collider = block.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                // BoxCollider2D size is affected by transform.scale automatically
                // But we can fine-tune if needed
            }
        }
    }

    void SetupBlockPhysics(GameObject block)
    {
        Rigidbody2D rb = block.GetComponent<Rigidbody2D>();
        BoxCollider2D collider = block.GetComponent<BoxCollider2D>();

        if (rb != null)
        {
            rb.mass = blockMass;
            rb.linearDamping = 0.1f; // Slight air resistance
            rb.angularDamping = 0.5f; // Prevent excessive spinning
        }

        if (collider != null && blockPhysicsMaterial != null)
        {
            collider.sharedMaterial = blockPhysicsMaterial;
        }

        // Add block cleanup component
        //block.AddComponent<BlockCleanup>();
    }

    // Visualize spawn area in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + (Vector3)spawnOffset;

        // Draw spawn range
        Gizmos.DrawWireCube(
            new Vector3(center.x, center.y + spawnHeight, 0),
            new Vector3(spawnRangeX, 1f, 0)
        );

        // Draw spawn height line
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(center.x - spawnRangeX / 2f, center.y + spawnHeight, 0),
            new Vector3(center.x + spawnRangeX / 2f, center.y + spawnHeight, 0)
        );
    }
}