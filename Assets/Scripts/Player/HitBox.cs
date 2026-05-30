using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private float damage;
    private string attackType;
    public Vector2 boxSize = new Vector2(1.5f, 1.5f);

    public void SetDamage(float dmg, string type)
    {
        damage = dmg;
        attackType = type;
    }

    public void CheckHit()
    {
        // Manually check for overlapping colliders
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            boxSize,
            0f
        );

        foreach (Collider2D hit in hits)
        {

            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth =
                    hit.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);

                    CombatController combat =
                        GetComponentInParent<CombatController>();
                    if (combat != null)
                        combat.GainSpecialMeter(
                            combat.specialMeterGainPerHit
                        );

                    Debug.Log(attackType + " hit: " +
                              hit.gameObject.name +
                              " for " + damage);
                }
            }
        }
    }

    // Visualize the hitbox in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}