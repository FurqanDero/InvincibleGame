using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private HUDManager hudManager;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        hudManager = Object.FindAnyObjectByType<HUDManager>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPlayerHurt();

        // ─── Screen distortion on damage ──────────
        if (PostProcessController.Instance != null)
            PostProcessController.Instance.TriggerDamageEffect();

        if (hudManager != null)
            hudManager.TakeDamage(damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            if (!isDead) // only reset color if still alive
                sr.color = Color.blue;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Mark defeated!");

        // Stop all movement immediately
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Disable input scripts
        PlayerController pc = GetComponent<PlayerController>();
        CombatController cc = GetComponent<CombatController>();
        if (pc != null) pc.enabled = false;
        if (cc != null) cc.enabled = false;

        // Play death sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEnemyDeath();

        // Hide Mark after short delay
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // Flash red three times
        for (int i = 0; i < 3; i++)
        {
            if (sr != null) sr.color = Color.red;
            yield return new WaitForSecondsRealtime(0.15f);
            if (sr != null) sr.color = Color.clear;
            yield return new WaitForSecondsRealtime(0.15f);
        }

        // Hide the square completely
        if (sr != null) sr.enabled = false;

        // Trigger game over after sequence
        yield return new WaitForSecondsRealtime(0.3f);

        if (GameManager.Instance != null)
            GameManager.Instance.TriggerGameOver();
    }
}