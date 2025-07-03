using UnityEngine;

public class LavaRiser : MonoBehaviour
{
    [Header("Rising Lava Settings")]
    public float riseSpeed = 1.0f; // Initial speed
    public float scaleSpeed = 0.5f;
    public float startDelay = 3.0f;
    public float warningDistance = 5.0f; // Distance at which warning starts
    public float lavaAcceleration = 0.05f; // Speed increase per second

    [Header("Warning Visual")]
    public GameObject warningUI; // Assign a UI element or sprite for warning (optional)

    private bool rising = false;
    private Transform player;
    private bool gameOver = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (warningUI != null)
            warningUI.SetActive(false);

        Invoke(nameof(StartRising), startDelay);
    }

    void StartRising() => rising = true;

    void Update()
    {
        if (gameOver) return;
        if (rising)
        {
            // Gradually increase the lava's speed
            riseSpeed += lavaAcceleration * Time.deltaTime;
            // Move up
            //transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            // Grow taller
            transform.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);
        }

        // Visual warning as lava approaches
        if (warningUI != null && player != null)
        {
            float distance = player.position.y - transform.position.y;
            warningUI.SetActive(distance < warningDistance);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameOver) return;
        if (other.CompareTag("Player"))
        {
            Animator anim = other.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("isHurt", true);
            }

            // Game Over logic
            gameOver = true;
            GameOverManager.Instance.GameOver(); // Assuming you have a GameOverManager instance
            Debug.Log("Game Over! Lava touched the player.");
            // Example: Destroy(other.gameObject);
            // Or: Call your GameManager.GameOver() method here
        }
    }
}