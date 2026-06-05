using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public static EnemyTracker Instance;

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
        // Count all enemies in scene at start
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
        Debug.Log("ALL ENEMIES DEFEATED — VICTORY!");

        VictoryUI victoryUI =
            Object.FindAnyObjectByType<VictoryUI>();
        if (victoryUI != null)
            victoryUI.Show();
    }
}