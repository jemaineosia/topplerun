using UnityEngine;

/// <summary>
/// Helper script to quickly set up a basic LavaRiser test environment
/// Attach this to an empty GameObject and run in Play mode to create test objects
/// </summary>
public class LavaRiserTestSetup : MonoBehaviour
{
    [Header("Test Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private Material lavaMaterial;
    [SerializeField] private Material platformMaterial;
    [SerializeField] private Material playerMaterial;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupTestEnvironment();
        }
    }
    
    /// <summary>
    /// Creates a basic test environment with lava, platforms, and a simple player
    /// </summary>
    [ContextMenu("Setup Test Environment")]
    public void SetupTestEnvironment()
    {
        // Create lava
        GameObject lava = CreateLava();
        
        // Create some platforms
        CreatePlatform("Ground Platform", new Vector3(0, -8, 0), new Vector3(20, 2, 1));
        CreatePlatform("Platform 1", new Vector3(-5, -4, 0), new Vector3(4, 1, 1));
        CreatePlatform("Platform 2", new Vector3(5, 0, 0), new Vector3(4, 1, 1));
        CreatePlatform("Platform 3", new Vector3(-3, 4, 0), new Vector3(4, 1, 1));
        CreatePlatform("Platform 4", new Vector3(3, 8, 0), new Vector3(4, 1, 1));
        
        // Create simple player
        GameObject player = CreatePlayer();
        
        // Connect lava to player
        LavaRiser lavaRiser = lava.GetComponent<LavaRiser>();
        if (lavaRiser != null && player != null)
        {
            lavaRiser.SetPlayer(player.transform);
        }
        
        // Create camera if none exists
        if (Camera.main == null)
        {
            CreateCamera(player);
        }
        
        // Add game manager if none exists
        if (FindObjectOfType<GameManager>() == null)
        {
            GameObject gameManagerObj = new GameObject("Game Manager");
            gameManagerObj.AddComponent<GameManager>();
        }
        
        Debug.Log("LavaRiser test environment created! Player can move with arrow keys or WASD.");
    }
    
    private GameObject CreateLava()
    {
        GameObject lava = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lava.name = "Lava";
        lava.transform.position = new Vector3(0, -12, 0);
        lava.transform.localScale = new Vector3(25, 4, 1);
        
        // Set up collider as trigger
        Collider collider = lava.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        BoxCollider2D lavaCollider = lava.AddComponent<BoxCollider2D>();
        lavaCollider.isTrigger = true;
        
        // Add LavaRiser script
        LavaRiser lavaRiser = lava.AddComponent<LavaRiser>();
        
        // Set up visual
        Renderer renderer = lava.GetComponent<Renderer>();
        if (renderer != null && lavaMaterial != null)
        {
            renderer.material = lavaMaterial;
        }
        else if (renderer != null)
        {
            renderer.material.color = new Color(1f, 0.3f, 0f, 0.8f); // Orange-red lava color
        }
        
        return lava;
    }
    
    private GameObject CreatePlatform(string name, Vector3 position, Vector3 scale)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = name;
        platform.transform.position = position;
        platform.transform.localScale = scale;
        platform.layer = LayerMask.NameToLayer("Default"); // Use default layer as platform layer
        
        // Remove 3D collider and add 2D collider
        Collider collider = platform.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        platform.AddComponent<BoxCollider2D>();
        
        // Set up visual
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null && platformMaterial != null)
        {
            renderer.material = platformMaterial;
        }
        else if (renderer != null)
        {
            renderer.material.color = Color.gray;
        }
        
        return platform;
    }
    
    private GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0, -6, 0);
        player.transform.localScale = new Vector3(1, 1, 1);
        
        // Remove 3D collider and add 2D collider
        Collider collider = player.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        player.AddComponent<BoxCollider2D>();
        
        // Add Rigidbody2D for physics
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        
        // Add simple movement script
        player.AddComponent<SimplePlayerMovement>();
        
        // Set up visual
        Renderer renderer = player.GetComponent<Renderer>();
        if (renderer != null && playerMaterial != null)
        {
            renderer.material = playerMaterial;
        }
        else if (renderer != null)
        {
            renderer.material.color = Color.blue;
        }
        
        return player;
    }
    
    private void CreateCamera(GameObject player)
    {
        GameObject cameraObj = new GameObject("Main Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.tag = "MainCamera";
        
        // Position camera
        cameraObj.transform.position = new Vector3(0, 0, -10);
        
        // Add simple follow script
        SimpleCameraFollow cameraFollow = cameraObj.AddComponent<SimpleCameraFollow>();
        cameraFollow.target = player.transform;
    }
}

/// <summary>
/// Simple player movement for testing LavaRiser
/// </summary>
public class SimplePlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        // Horizontal movement
        float horizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            isGrounded = true;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            isGrounded = false;
        }
    }
}

/// <summary>
/// Simple camera follow for testing
/// </summary>
public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}