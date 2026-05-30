using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Attack Settings")]
    public float punchDamage = 10f;
    public float kickDamage = 15f;
    public float aerialSlamDamage = 25f;
    public float specialDamage = 40f;

    [Header("Attack Duration")]
    public float attackDuration = 0.5f;
    private float attackTimer = 0f;
    private bool isAttacking = false;

    [Header("Special Meter")]
    public float maxSpecialMeter = 100f;
    public float currentSpecialMeter = 0f;
    public float specialMeterGainPerHit = 15f;

    [Header("References")]
    public GameObject hitbox;
    public CameraShake cameraShake;

    private string currentAttack = "";
    private PlayerController playerController;
    private Hitbox hitboxScript;
    private BoxCollider2D hitboxCollider;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        hitboxScript = hitbox.GetComponent<Hitbox>();
        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();
        hitboxCollider.enabled = false;
    }

    void Update()
    {
        HandleAttackInput();
        HandleAttackTimer();
    }

    void HandleAttackInput()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.J) &&
            Input.GetKey(KeyCode.S) &&
            !playerController.isGrounded)
        {
            StartAttack("AerialSlam", aerialSlamDamage);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            StartAttack("Punch", punchDamage);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            StartAttack("Kick", kickDamage);
        }
        else if (Input.GetKeyDown(KeyCode.L) &&
                 currentSpecialMeter >= maxSpecialMeter)
        {
            StartAttack("Special", specialDamage);
            currentSpecialMeter = 0f;
        }
    }

    void StartAttack(string attackName, float damage)
    {
        isAttacking = true;
        currentAttack = attackName;
        attackTimer = attackDuration;

        hitboxScript.SetDamage(damage, attackName);
        hitboxCollider.enabled = true;
        hitboxScript.CheckHit();

        if (attackName == "AerialSlam" || attackName == "Special")
        {
            if (cameraShake != null)
                cameraShake.Shake(0.3f, 0.2f);
        }

    }

    void HandleAttackTimer()
    {
        if (!isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            isAttacking = false;
            hitboxCollider.enabled = false;
            currentAttack = "";
        }
    }

    public void GainSpecialMeter(float amount)
    {
        currentSpecialMeter = Mathf.Min(
            currentSpecialMeter + amount,
            maxSpecialMeter
        );
    }
}