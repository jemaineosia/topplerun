using UnityEngine;

public class BlockCleanup : MonoBehaviour
{
    [Header("Cleanup Settings")]
    public float cleanupDistance = 50f; // Distance below camera before cleanup
    public float maxLifetime = 60f; // Maximum time before auto-cleanup

    private Camera mainCamera;
    private float spawnTime;

    void Start()
    {
        mainCamera = Camera.main;
        spawnTime = Time.time;
    }

    void Update()
    {
        // Check if block is too far below camera
        if (mainCamera != null)
        {
            float cameraBottom = mainCamera.transform.position.y - (mainCamera.orthographicSize + cleanupDistance);
            if (transform.position.y < cameraBottom)
            {
                DestroyBlock();
                return;
            }
        }

        // Check max lifetime
        if (Time.time - spawnTime > maxLifetime)
        {
            DestroyBlock();
        }
    }

    void DestroyBlock()
    {
        Destroy(gameObject);
    }
}