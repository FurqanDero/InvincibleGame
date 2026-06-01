using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // ─── State Machine ───────────────────────────
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Hurt,
        Death
    }

    public EnemyState currentState = EnemyState.Patrol;

    // ─── Detection ───────────────────────────────
    [Header("Detection")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;
    private Transform player;

    // ─── Movement ────────────────────────────────
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolDistance = 4f;
    private Vector2 startPosition;
    private bool movingRight = true;

    // ─── Attack ──────────────────────────────────
    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;

    // ─── Hurt ────────────────────────────────────
    [Header("Hurt")]
    public float hurtDuration = 0.3f;
    private float hurtTimer = 0f;

    // ─── References ──────────────────────────────
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private EnemyHealth enemyHealth;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        startPosition = transform.position;

        // Find player
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (currentState == EnemyState.Death) return;

        // Stop if game is over
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
            case EnemyState.Hurt:
                UpdateHurt();
                break;
        }

        CheckForPlayer();
    }

    // ─── PATROL ──────────────────────────────────
    void UpdatePatrol()
    {
        // Move back and forth
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(patrolSpeed, rb.linearVelocity.y);

            if (transform.position.x >= startPosition.x + patrolDistance)
                movingRight = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-patrolSpeed, rb.linearVelocity.y);

            if (transform.position.x <= startPosition.x - patrolDistance)
                movingRight = true;
        }

        FlipToMovement();
    }

    // ─── CHASE ───────────────────────────────────
    void UpdateChase()
    {
        float distanceToPlayer = Vector2.Distance(
            transform.position,
            player.position
        );

        // If player out of range, go back to patrol
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        // If close enough, attack
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Chase the player
        Vector2 direction = (player.position -
                            transform.position).normalized;
        rb.linearVelocity = new Vector2(
            direction.x * chaseSpeed,
            rb.linearVelocity.y
        );

        FlipToTarget(player.position);
    }

    // ─── ATTACK ──────────────────────────────────
    void UpdateAttack()
    {
        float distanceToPlayer = Vector2.Distance(
            transform.position,
            player.position
        );

        // Player moved away
        if (distanceToPlayer > attackRange * 1.5f)
        {
            currentState = EnemyState.Chase;
            return;
        }

        // Attack cooldown
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }

        rb.linearVelocity = Vector2.zero;
    }

    void PerformAttack()
    {
        // Deal damage to player
        PlayerHealth playerHealth = player
            .GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked Mark for " + attackDamage);
        }
    }

    // ─── HURT ────────────────────────────────────
    void UpdateHurt()
    {
        hurtTimer -= Time.deltaTime;

        if (hurtTimer <= 0f)
        {
            sr.color = Color.red;
            currentState = EnemyState.Chase;
        }
    }

    public void TriggerHurt(Vector2 knockbackDirection,
                             float knockbackForce)
    {
        if (currentState == EnemyState.Death) return;

        currentState = EnemyState.Hurt;
        hurtTimer = hurtDuration;

        // Flash white
        sr.color = Color.white;

        // Apply knockback
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce,
                    ForceMode2D.Impulse);
    }

    // ─── DEATH ───────────────────────────────────
    public void TriggerDeath()
    {
        currentState = EnemyState.Death;
        rb.linearVelocity = Vector2.zero;
        sr.color = Color.grey;

        // Disable collider
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 1f);
        Debug.Log(gameObject.name + " defeated!");
    }

    // ─── DETECTION ───────────────────────────────
    void CheckForPlayer()
    {
        if (currentState == EnemyState.Patrol)
        {
            float distanceToPlayer = Vector2.Distance(
                transform.position,
                player.position
            );

            if (distanceToPlayer <= detectionRange)
            {
                currentState = EnemyState.Chase;
                Debug.Log(gameObject.name + " detected Mark!");
            }
        }
    }

    // ─── HELPERS ─────────────────────────────────
    void FlipToMovement()
    {
        if (movingRight && !facingRight)
            Flip();
        else if (!movingRight && facingRight)
            Flip();
    }

    void FlipToTarget(Vector3 target)
    {
        bool shouldFaceRight = target.x > transform.position.x;
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

    // ─── GIZMOS ──────────────────────────────────
    void OnDrawGizmosSelected()
    {
        // Detection range — yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range — red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}