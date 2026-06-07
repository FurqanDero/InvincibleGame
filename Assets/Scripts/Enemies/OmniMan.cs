using UnityEngine;
using System.Collections;

public class OmniMan : MonoBehaviour
{
    // ─── Phase ───────────────────────────────────
    public enum BossPhase { Idle, Phase1, Phase2, Dead }
    public BossPhase currentPhase = BossPhase.Idle;

    // ─── State ───────────────────────────────────
    public enum BossState
    {
        Idle, Walk, Punch, Slam,
        Charge, Shockwave, Hurt, Dead
    }
    public BossState currentState = BossState.Idle;

    // ─── Stats ───────────────────────────────────
    [Header("Health")]
    public float maxHealth = 200f;
    private float currentHealth;
    public float phase2Threshold = 100f;
    private bool phase2Triggered = false;

    [Header("Phase 1 — Ground")]
    public float walkSpeed = 3f;
    public float punchRange = 2f;
    public float punchDamage = 20f;
    public float punchCooldown = 1.5f;
    public float slamDamage = 35f;
    public float slamRange = 2.5f;
    public float slamCooldown = 4f;

    [Header("Phase 2 — Aerial")]
    public float chargeSpeed = 18f;
    public float chargeDamage = 25f;
    public float shockwaveDamage = 15f;
    public float shockwaveRange = 4f;
    public float phase2AttackCooldown = 2f;

    [Header("Knockback")]
    public float knockbackForce = 12f;

    // ─── Timers ──────────────────────────────────
    private float punchTimer = 0f;
    private float slamTimer = 0f;
    private float phase2Timer = 0f;
    private float hurtTimer = 0f;
    private float hurtDuration = 0.4f;

    // ─── References ──────────────────────────────
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform player;
    private PlayerHealth playerHealth;
    private CameraShake cameraShake;
    private bool facingRight = false;
    private bool isCharging = false;

    // ─── Boss Health Bar ─────────────────────────
    [Header("Boss HUD")]
    public BossHealthBar bossHealthBar;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        player = GameObject.FindWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        cameraShake = Object.FindAnyObjectByType<CameraShake>();

        // Auto-start fight after short delay
        Invoke("StartFight", 2f);
    }

    void Update()
    {
        if (currentPhase == BossPhase.Idle) return;
        if (currentPhase == BossPhase.Dead) return;

        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentPhase)
        {
            case BossPhase.Phase1:
                UpdatePhase1();
                break;
            case BossPhase.Phase2:
                UpdatePhase2();
                break;
        }
    }

    // ─── PHASE 1 ─────────────────────────────────
    void UpdatePhase1()
    {
        if (currentState == BossState.Hurt)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
            {
                sr.color = new Color(0.8f, 0.7f, 0f);
                currentState = BossState.Walk;
            }
            return;
        }

        float distToPlayer = Vector2.Distance(
            transform.position, player.position);

        FlipToTarget(player.position);

        // Update timers
        punchTimer -= Time.deltaTime;
        slamTimer -= Time.deltaTime;

        // Slam takes priority if in range and ready
        if (distToPlayer <= slamRange && slamTimer <= 0)
        {
            StartCoroutine(PerformSlam());
            slamTimer = slamCooldown;
            return;
        }

        // Punch if in range
        if (distToPlayer <= punchRange && punchTimer <= 0)
        {
            StartCoroutine(PerformPunch());
            punchTimer = punchCooldown;
            return;
        }

        // Walk toward player
        currentState = BossState.Walk;
        Vector2 dir = (player.position -
                      transform.position).normalized;
        rb.linearVelocity = new Vector2(
            dir.x * walkSpeed,
            rb.linearVelocity.y
        );
    }

    IEnumerator PerformPunch()
    {
        currentState = BossState.Punch;
        rb.linearVelocity = Vector2.zero;

        // Wind up
        sr.color = Color.white;
        yield return new WaitForSeconds(0.3f);

        // Strike
        sr.color = new Color(0.8f, 0.7f, 0f);

        float dist = Vector2.Distance(
            transform.position, player.position);

        if (dist <= punchRange + 0.5f)
        {
            Vector2 knockDir = (player.position -
                               transform.position).normalized;
            knockDir.y = 0.3f;

            playerHealth?.TakeDamage(punchDamage);

            Rigidbody2D playerRb =
                player.GetComponent<Rigidbody2D>();
            playerRb?.AddForce(
                knockDir * knockbackForce,
                ForceMode2D.Impulse
            );

            if (cameraShake != null)
                cameraShake.Shake(0.4f, 0.25f);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPlayerHurt();
        }

        yield return new WaitForSeconds(0.2f);
        currentState = BossState.Walk;
    }

    IEnumerator PerformSlam()
    {
        currentState = BossState.Slam;
        rb.linearVelocity = Vector2.zero;

        // Wind up — dramatic pause
        sr.color = Color.white;
        yield return new WaitForSeconds(0.5f);

        // SLAM
        sr.color = new Color(0.8f, 0.7f, 0f);

        float dist = Vector2.Distance(
            transform.position, player.position);

        if (dist <= slamRange + 0.5f)
        {
            playerHealth?.TakeDamage(slamDamage);

            // Launch player upward
            Rigidbody2D playerRb =
                player.GetComponent<Rigidbody2D>();
            playerRb?.AddForce(
                new Vector2(0, 18f),
                ForceMode2D.Impulse
            );

            // Heavy screen shake
            if (cameraShake != null)
                cameraShake.Shake(0.8f, 0.4f);

            if (PostProcessController.Instance != null)
                PostProcessController.Instance
                    .TriggerDamageEffect();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySlam();
        }

        yield return new WaitForSeconds(0.3f);
        currentState = BossState.Walk;
    }

    // ─── PHASE 2 ─────────────────────────────────
    void UpdatePhase2()
    {
        if (currentState == BossState.Hurt)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
            {
                sr.color = new Color(0.8f, 0.7f, 0f);
                currentState = BossState.Idle;
            }
            return;
        }

        if (isCharging) return;

        phase2Timer -= Time.deltaTime;

        if (phase2Timer <= 0)
        {
            // Alternate between charge and shockwave
            if (Random.value > 0.5f)
                StartCoroutine(PerformCharge());
            else
                StartCoroutine(PerformShockwave());

            phase2Timer = phase2AttackCooldown;
        }

        FlipToTarget(player.position);
    }

    IEnumerator PerformCharge()
    {
        isCharging = true;
        currentState = BossState.Charge;

        // Float up
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        sr.color = Color.white;
        yield return new WaitForSeconds(0.6f);

        // Charge across screen toward player
        sr.color = new Color(0.8f, 0.7f, 0f);
        Vector2 chargeDir = (player.position -
                            transform.position).normalized;

        float chargeTime = 0.4f;
        float elapsed = 0f;

        while (elapsed < chargeTime)
        {
            rb.linearVelocity = chargeDir * chargeSpeed;
            elapsed += Time.deltaTime;

            // Hit player during charge
            float dist = Vector2.Distance(
                transform.position, player.position);
            if (dist < 1.5f)
            {
                playerHealth?.TakeDamage(chargeDamage);

                Rigidbody2D playerRb =
                    player.GetComponent<Rigidbody2D>();
                playerRb?.AddForce(
                    chargeDir * 15f,
                    ForceMode2D.Impulse
                );

                if (cameraShake != null)
                    cameraShake.Shake(0.6f, 0.3f);

                break;
            }

            yield return null;
        }

        // Reset after charge
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        yield return new WaitForSeconds(0.4f);
        isCharging = false;
        currentState = BossState.Idle;
    }

    IEnumerator PerformShockwave()
    {
        currentState = BossState.Shockwave;
        rb.linearVelocity = Vector2.zero;

        sr.color = Color.white;
        yield return new WaitForSeconds(0.4f);

        sr.color = new Color(0.8f, 0.7f, 0f);

        // Check if player is in shockwave range
        float dist = Vector2.Distance(
            transform.position, player.position);

        if (dist <= shockwaveRange)
        {
            playerHealth?.TakeDamage(shockwaveDamage);

            if (cameraShake != null)
                cameraShake.Shake(0.5f, 0.35f);

            if (PostProcessController.Instance != null)
                PostProcessController.Instance
                    .TriggerDamageEffect();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySlam();
        }

        yield return new WaitForSeconds(0.3f);
        currentState = BossState.Idle;
    }

    // ─── PHASE TRANSITION ────────────────────────
    IEnumerator TriggerPhase2Transition()
    {
        currentState = BossState.Idle;
        rb.linearVelocity = Vector2.zero;
        phase2Triggered = true;

        // Dramatic pause
        if (cameraShake != null)
            cameraShake.Shake(0.6f, 0.5f);

        sr.color = Color.white;
        yield return new WaitForSeconds(0.3f);
        sr.color = new Color(0.8f, 0.7f, 0f);
        yield return new WaitForSeconds(0.3f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.3f);
        sr.color = new Color(0.8f, 0.7f, 0f);

        yield return new WaitForSeconds(0.5f);

        // Switch to aerial phase
        currentPhase = BossPhase.Phase2;
        currentState = BossState.Idle;
        rb.gravityScale = 0f;
        phase2Timer = 1f;

        Debug.Log("OMNI-MAN PHASE 2!");
    }

    // ─── TAKE DAMAGE ─────────────────────────────
    public void TakeDamage(float damage,
                            Vector2 knockbackDir)
    {
        if (currentPhase == BossPhase.Idle) return;
        if (currentPhase == BossPhase.Dead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        // Update boss health bar
        if (bossHealthBar != null)
            bossHealthBar.UpdateHealth(
                currentHealth, maxHealth);

        // Hurt flash
        StopAllCoroutines();
        isCharging = false;
        currentState = BossState.Hurt;
        hurtTimer = hurtDuration;
        sr.color = Color.white;

        // Knockback — reduced for boss
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * 3f,
                    ForceMode2D.Impulse);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEnemyHurt();

        // Phase 2 trigger
        if (!phase2Triggered &&
            currentHealth <= phase2Threshold)
        {
            StartCoroutine(TriggerPhase2Transition());
            return;
        }

        // Death
        if (currentHealth <= 0)
            StartCoroutine(TriggerDeath());
    }

    IEnumerator TriggerDeath()
    {
        currentPhase = BossPhase.Dead;
        currentState = BossState.Dead;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;

        // Death sequence — flash rapidly
        for (int i = 0; i < 6; i++)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(0.8f, 0.7f, 0f);
            yield return new WaitForSeconds(0.1f);
        }

        sr.color = Color.grey;
        GetComponent<Collider2D>().enabled = false;

        if (cameraShake != null)
            cameraShake.Shake(1f, 0.5f);

        yield return new WaitForSeconds(1f);

        // Trigger victory
        VictoryUI victoryUI =
            Object.FindAnyObjectByType<VictoryUI>();
        if (victoryUI != null)
            victoryUI.Show();

        Destroy(gameObject, 0.5f);
    }

    // ─── ENTRANCE ────────────────────────────────
    public void StartFight()
    {
        currentPhase = BossPhase.Phase1;
        currentState = BossState.Walk;

        if (bossHealthBar != null)
            bossHealthBar.gameObject.SetActive(true);

        Debug.Log("OMNI-MAN FIGHT STARTED!");
    }

    // ─── HELPERS ─────────────────────────────────
    void FlipToTarget(Vector3 target)
    {
        bool shouldFaceRight =
            target.x > transform.position.x;
        if (shouldFaceRight && !facingRight)
            Flip();
        else if (!shouldFaceRight && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, punchRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, slamRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.position, shockwaveRange);
    }
}