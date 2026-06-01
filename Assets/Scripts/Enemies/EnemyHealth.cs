using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 8f;

    private EnemyAI enemyAI;
    private FlaxanSoldier flaxanSoldier;

    void Start()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>();
        flaxanSoldier = GetComponent<FlaxanSoldier>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        // Don't damage dead enemies
        if (enemyAI != null &&
            enemyAI.currentState == EnemyAI.EnemyState.Death)
            return;
        if (flaxanSoldier != null &&
            flaxanSoldier.currentState == FlaxanSoldier.FlaxanState.Death)
            return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " HP: " + currentHealth);

        // Trigger hurt on whichever AI is present
        if (enemyAI != null)
            enemyAI.TriggerHurt(knockbackDirection, knockbackForce);
        else if (flaxanSoldier != null)
            flaxanSoldier.TriggerHurt(knockbackDirection, knockbackForce);

        if (currentHealth <= 0)
        {
            if (enemyAI != null)
                enemyAI.TriggerDeath();
            else if (flaxanSoldier != null)
                flaxanSoldier.TriggerDeath();
            else
                Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, Vector2.right);
    }
}