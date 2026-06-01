using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private HUDManager hudManager;

    void Start()
    {
        currentHealth = maxHealth;
        hudManager = Object.FindAnyObjectByType<HUDManager>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0f);

        if (hudManager != null)
            hudManager.TakeDamage(damage);

        StartCoroutine(DamageFlash());

        Debug.Log("Mark HP: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    System.Collections.IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            sr.color = Color.blue;
        }
    }

    void Die()
    {
        Debug.Log("Mark defeated!");
    }
}