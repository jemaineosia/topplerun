using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Cloud Settings")]
    public Sprite[] cloudSprites;          // Assign multiple cloud sprites
    public GameObject cloudPrefab;         // A prefab with SpriteRenderer & no sprite assigned
    public int minClouds = 3;
    public int maxClouds = 7;
    public float minY = 0f;                // Lower bound for cloud Y
    public float maxY = 10f;               // Upper bound for cloud Y
    public float minSpeed = 0.5f;          // Min cloud speed
    public float maxSpeed = 2f;            // Max cloud speed
    public float minScale = 0.5f;          // Min cloud scale
    public float maxScale = 1.5f;          // Max cloud scale
    public float spawnPadding = 2f;        // Offscreen spawn padding
    [Header("Visual Settings")]
    public float parallaxFactor = 1f;      // Cloud movement speed multiplier
    public Color cloudColor = Color.white;  // Cloud tint color

    [Header("Area")]
    public Camera mainCamera;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        int cloudCount = Random.Range(minClouds, maxClouds + 1);

        for (int i = 0; i < cloudCount; i++)
        {
            SpawnCloud();
        }
    }

    void SpawnCloud()
    {
        // Pick a random sprite
        Sprite sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];

        // Get camera edges in world
        float camWidth = mainCamera.orthographicSize * mainCamera.aspect * 2f;
        float minX = mainCamera.transform.position.x - camWidth / 2f - spawnPadding;
        float maxX = mainCamera.transform.position.x + camWidth / 2f + spawnPadding;

        // Random starting side (left or right)
        bool fromLeft = Random.value > 0.5f;
        float x = fromLeft ? minX : maxX;
        float y = Random.Range(minY, maxY);

        // Instantiate and configure cloud
        GameObject cloud = Instantiate(cloudPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
        SpriteRenderer sr = cloud.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = -10;

        // Random scale
        float scale = Random.Range(minScale, maxScale);
        cloud.transform.localScale = Vector3.one * scale;

        // Assign random speed and direction
        float speed = Random.Range(minSpeed, maxSpeed) * (fromLeft ? 1 : -1);
        cloud.AddComponent<CloudMover>().Init(speed, minX, maxX, y, spawnPadding, parallaxFactor, cloudColor);
    }
}