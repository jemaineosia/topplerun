using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Example Game Manager that demonstrates integration with LavaRiser system
/// This script shows how to handle the game over event from LavaRiser
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Over Settings")]
    [SerializeField] private bool autoRestartOnGameOver = true;
    [SerializeField] private float restartDelay = 2f;
    
    void Start()
    {
        // Subscribe to the LavaRiser game over event
        LavaRiser.OnGameOver += HandleGameOver;
        
        // Ensure normal time scale
        Time.timeScale = 1f;
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        LavaRiser.OnGameOver -= HandleGameOver;
    }
    
    /// <summary>
    /// Handles game over event from LavaRiser
    /// </summary>
    private void HandleGameOver()
    {
        Debug.Log("Game Manager: Handling game over from LavaRiser");
        
        if (autoRestartOnGameOver)
        {
            Invoke(nameof(RestartGame), restartDelay);
        }
        else
        {
            // Pause the game and wait for player input
            Time.timeScale = 0f;
            Debug.Log("Game Over! Press R to restart or implement your own game over UI.");
        }
    }
    
    /// <summary>
    /// Restarts the current scene
    /// </summary>
    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void Update()
    {
        // Example: Manual restart with R key (useful during development)
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // Example: Pause/unpause with P key
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = Time.timeScale == 0 ? 1f : 0f;
        }
    }
}