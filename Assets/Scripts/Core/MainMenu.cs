using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayPressed()
    {
        SceneManager.LoadScene("Level_01");
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}