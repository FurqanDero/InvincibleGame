using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float damage = 15f;
    public float lifetime = 3f;

    private Vector2 direction;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;

        // Auto destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Destroy(gameObject);
        }

        // Hit ground — destroy
        if (other.gameObject.layer ==
            LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}