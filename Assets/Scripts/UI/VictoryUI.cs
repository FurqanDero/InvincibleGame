using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    public GameObject victoryPanel;

    void Start()
    {
        victoryPanel.SetActive(false);
    }

    public void Show()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnRestartPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level_01");
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}