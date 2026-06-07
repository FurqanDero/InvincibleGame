using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTracker : MonoBehaviour
{
    public static EnemyTracker Instance;
    public bool hasBossScene = false;

    private int totalEnemies = 0;
    private int defeatedEnemies = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        totalEnemies = GameObject
            .FindGameObjectsWithTag("Enemy").Length;
        Debug.Log("Total enemies: " + totalEnemies);
    }

    public void EnemyDefeated()
    {
        defeatedEnemies++;
        Debug.Log("Defeated: " + defeatedEnemies +
                  "/" + totalEnemies);

        if (defeatedEnemies >= totalEnemies)
            TriggerVictory();
    }

    void TriggerVictory()
    {
        // Check for boss in current scene
        OmniMan omniMan =
            Object.FindAnyObjectByType<OmniMan>();

        if (omniMan != null)
        {
            omniMan.StartFight();
            return;
        }

        // Load boss scene if flagged
        if (hasBossScene)
        {
            SceneManager.LoadScene("BossScene");
            return;
        }

        // Normal victory
        VictoryUI victoryUI =
            Object.FindAnyObjectByType<VictoryUI>();
        if (victoryUI != null)
            victoryUI.Show();
    }
}