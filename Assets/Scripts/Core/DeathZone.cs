using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private Vector3 respawnPoint;

    void Start()
    {
        // Save Mark's starting position as respawn point
        respawnPoint = GameObject.FindWithTag("Player").transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = respawnPoint;

            // Reset velocity so he doesn't keep falling after respawn
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }
}