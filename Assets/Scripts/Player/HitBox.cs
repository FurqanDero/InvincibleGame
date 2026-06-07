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
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position, boxSize, 0f
        );

        foreach (Collider2D hit in hits)
        {
            // Hit regular enemy
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth =
                    hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    Vector2 knockbackDir =
                        (hit.transform.position -
                        transform.root.position).normalized;

                    if (attackType == "AerialSlam")
                        knockbackDir = new Vector2(
                            knockbackDir.x, 1f).normalized;

                    enemyHealth.TakeDamage(damage, knockbackDir);

                    CombatController combat =
                        GetComponentInParent<CombatController>();
                    if (combat != null)
                        combat.GainSpecialMeter(
                            combat.specialMeterGainPerHit);
                }
            }

            // Hit Omni-Man boss
            if (hit.CompareTag("Boss"))
            {
                OmniMan omniMan =
                    hit.GetComponent<OmniMan>();
                if (omniMan != null)
                {
                    Vector2 knockbackDir =
                        (hit.transform.position -
                        transform.root.position).normalized;

                    omniMan.TakeDamage(damage, knockbackDir);

                    CombatController combat =
                        GetComponentInParent<CombatController>();
                    if (combat != null)
                        combat.GainSpecialMeter(
                            combat.specialMeterGainPerHit);
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