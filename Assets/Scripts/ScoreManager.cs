using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public TextMeshProUGUI scoreText;
    public float scoreRate = 1f; // Points per second

    private float score = 0f;
    private bool isAlive = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ResetScore();
    }

    void Update()
    {
        if (isAlive)
        {
            score += scoreRate * Time.deltaTime;
            UpdateScoreDisplay();
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Time: " + Mathf.FloorToInt(score);
        }
    }

    // Call this when the player dies
    public void StopScoring()
    {
        isAlive = false;
    }

    public void ResetScore()
    {
        score = 0f;
        isAlive = true;
        UpdateScoreDisplay();
    }

    public int GetScore()
    {
        return Mathf.FloorToInt(score);
    }
}
