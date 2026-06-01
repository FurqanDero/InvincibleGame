using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private EnemyAI enemyAI;

    [Header("Knockback")]
    public float knockbackForce = 8f;

    void Start()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if (enemyAI != null &&
            enemyAI.currentState == EnemyAI.EnemyState.Death)
            return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " HP: " + currentHealth);

        // Trigger hurt state with knockback
        if (enemyAI != null)
            enemyAI.TriggerHurt(knockbackDirection, knockbackForce);

        if (currentHealth <= 0)
        {
            if (enemyAI != null)
                enemyAI.TriggerDeath();
            else
                Destroy(gameObject);
        }
    }

    // Overload without knockback for backwards compatibility
    public void TakeDamage(float damage)
    {
        TakeDamage(damage, Vector2.right);
    }
}