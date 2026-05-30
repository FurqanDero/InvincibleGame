using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage +
                  " damage. HP: " + currentHealth);

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
            Die();
    }

    System.Collections.IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.red;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " defeated!");
        Destroy(gameObject);
    }
}