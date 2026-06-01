using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void Show()
    {
        gameOverPanel.SetActive(true);
    }

    public void OnRestartPressed()
    {
        GameManager.Instance.RestartGame();
    }

    public void OnQuitPressed()
    {
        GameManager.Instance.QuitGame();
    }
}