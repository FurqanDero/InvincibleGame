using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("HP Bar")]
    public RectTransform hpBarFill;
    public float maxHP = 100f;
    private float currentHP;
    private float hpBarMaxWidth;

    [Header("Special Meter")]
    public RectTransform specialBarFill;
    private float specialBarMaxWidth;
    private CombatController combatController;

    void Start()
    {
        currentHP = maxHP;

        // Save the full width as 100%
        hpBarMaxWidth = hpBarFill.sizeDelta.x;
        specialBarMaxWidth = specialBarFill.sizeDelta.x;

        combatController = GameObject
            .FindWithTag("Player")
            .GetComponent<CombatController>();
    }

    void Update()
    {
        UpdateSpecialBar();
    }

    void UpdateSpecialBar()
    {
        if (combatController == null) return;

        float ratio = combatController.currentSpecialMeter /
                      combatController.maxSpecialMeter;

        specialBarFill.sizeDelta = new Vector2(
            specialBarMaxWidth * ratio,
            specialBarFill.sizeDelta.y
        );
    }

    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0f);

        float ratio = currentHP / maxHP;

        hpBarFill.sizeDelta = new Vector2(
            hpBarMaxWidth * ratio,
            hpBarFill.sizeDelta.y
        );

        if (currentHP <= 0)
            PlayerDied();
    }

    void PlayerDied()
    {
        Debug.Log("Mark has died!");
    }
}