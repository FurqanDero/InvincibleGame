using UnityEngine;

public class FlaxanSoldier : MonoBehaviour
{
    public enum FlaxanState
    {
        Patrol,
        Chase,
        Shoot,
        BackAway,
        Hurt,
        Death
    }

    public FlaxanState currentState = FlaxanState.Patrol;

    [Header("Detection")]
    public float detectionRange = 12f;
    public float preferredShootRange = 6f;
    public float tooCloseRange = 3f;

    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float backAwaySpeed = 3f;
    public float patrolDistance = 3f;
    private Vector2 startPosition;
    private bool movingRight = true;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;
    private float shootTimer = 0f;

    [Header("Hurt")]
    public float hurtDuration = 0.3f;
    private float hurtTimer = 0f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform player;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (currentState == FlaxanState.Death) return;

        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(
            transform.position, player.position
        );

        switch (currentState)
        {
            case FlaxanState.Patrol:
                UpdatePatrol(distanceToPlayer);
                break;
            case FlaxanState.Chase:
                UpdateChase(distanceToPlayer);
                break;
            case FlaxanState.Shoot:
                UpdateShoot(distanceToPlayer);
                break;
            case FlaxanState.BackAway:
                UpdateBackAway(distanceToPlayer);
                break;
            case FlaxanState.Hurt:
                UpdateHurt();
                break;
        }
    }

    // ─── PATROL ──────────────────────────────────
    void UpdatePatrol(float distanceToPlayer)
    {
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(
                patrolSpeed, rb.linearVelocity.y
            );
            if (transform.position.x >=
                startPosition.x + patrolDistance)
                movingRight = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(
                -patrolSpeed, rb.linearVelocity.y
            );
            if (transform.position.x <=
                startPosition.x - patrolDistance)
                movingRight = true;
        }

        FlipToMovement();

        // Detect player
        if (distanceToPlayer <= detectionRange)
            currentState = FlaxanState.Chase;
    }

    // ─── CHASE ───────────────────────────────────
    void UpdateChase(float distanceToPlayer)
    {
        // Player escaped
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            currentState = FlaxanState.Patrol;
            return;
        }

        // Too close — back away
        if (distanceToPlayer < tooCloseRange)
        {
            currentState = FlaxanState.BackAway;
            return;
        }

        // At preferred range — shoot
        if (distanceToPlayer <= preferredShootRange)
        {
            currentState = FlaxanState.Shoot;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Move toward player
        Vector2 direction = (player.position -
                            transform.position).normalized;
        rb.linearVelocity = new Vector2(
            direction.x * chaseSpeed,
            rb.linearVelocity.y
        );

        FlipToTarget(player.position);
    }

    // ─── SHOOT ───────────────────────────────────
    void UpdateShoot(float distanceToPlayer)
    {
        rb.linearVelocity = Vector2.zero;
        FlipToTarget(player.position);

        // Player moved too far — chase again
        if (distanceToPlayer > preferredShootRange * 1.3f)
        {
            currentState = FlaxanState.Chase;
            return;
        }

        // Player too close — back away
        if (distanceToPlayer < tooCloseRange)
        {
            currentState = FlaxanState.BackAway;
            return;
        }

        // Shoot on cooldown
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            FireProjectile();
            shootTimer = shootCooldown;
        }
    }

    // ─── BACK AWAY ───────────────────────────────
    void UpdateBackAway(float distanceToPlayer)
    {
        // Back at preferred range — shoot
        if (distanceToPlayer >= preferredShootRange)
        {
            currentState = FlaxanState.Shoot;
            return;
        }

        // Move away from player
        Vector2 direction = (transform.position -
                            player.position).normalized;
        rb.linearVelocity = new Vector2(
            direction.x * backAwaySpeed,
            rb.linearVelocity.y
        );

        FlipToTarget(player.position);
    }

    // ─── HURT ────────────────────────────────────
    void UpdateHurt()
    {
        hurtTimer -= Time.deltaTime;
        if (hurtTimer <= 0f)
        {
            sr.color = new Color(0.6f, 0.8f, 1f);
            currentState = FlaxanState.Chase;
        }
    }

    public void TriggerHurt(Vector2 knockbackDirection,
                             float knockbackForce)
    {
        if (currentState == FlaxanState.Death) return;

        currentState = FlaxanState.Hurt;
        hurtTimer = hurtDuration;
        sr.color = Color.white;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce,
                    ForceMode2D.Impulse);
    }

    public void TriggerDeath()
    {
        currentState = FlaxanState.Death;
        rb.linearVelocity = Vector2.zero;
        sr.color = Color.grey;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
        Debug.Log(gameObject.name + " defeated!");
    }

    // ─── FIRE ────────────────────────────────────
    void FireProjectile()
    {
        if (projectilePrefab == null) return;

        // Spawn at fire point or self position
        Vector3 spawnPos = firePoint != null ?
            firePoint.position : transform.position;

        GameObject proj = Instantiate(
            projectilePrefab,
            spawnPos,
            Quaternion.identity
        );

        // Aim at player
        Vector2 direction = (player.position -
                            spawnPos).normalized;

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.SetDirection(direction);

        Debug.Log("Flaxan fired projectile!");
    }

    // ─── HELPERS ─────────────────────────────────
    void FlipToMovement()
    {
        if (movingRight && !facingRight) Flip();
        else if (!movingRight && facingRight) Flip();
    }

    void FlipToTarget(Vector3 target)
    {
        bool shouldFaceRight =
            target.x > transform.position.x;
        if (shouldFaceRight && !facingRight) Flip();
        else if (!shouldFaceRight && facingRight) Flip();
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

        // Preferred shoot range — blue
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, preferredShootRange);

        // Too close range — red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tooCloseRange);
    }
}