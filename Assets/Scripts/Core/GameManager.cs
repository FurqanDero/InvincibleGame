using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isGameOver = false;

    void Awake()
    {
        // Singleton — only one GameManager exists
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("GAME OVER");

        // Freeze time — stops all physics and Update loops
        Time.timeScale = 0f;

        // Show game over UI
        GameOverUI gameOverUI =
            Object.FindAnyObjectByType<GameOverUI>();
        if (gameOverUI != null)
            gameOverUI.Show();
    }

    public void RestartGame()
    {
        // Unfreeze time before reloading
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}