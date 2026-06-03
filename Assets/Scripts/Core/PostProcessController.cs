using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    public static PostProcessController Instance;

    private Volume volume;
    private ChromaticAberration chromaticAberration;
    private Bloom bloom;
    private Vignette vignette;

    [Header("Damage Effect")]
    public float damageAberrationIntensity = 0.8f;
    public float damageAberrationDuration = 0.3f;
    private float aberrationTimer = 0f;

    [Header("Special Effect")]
    public float specialBloomIntensity = 3f;
    public float normalBloomIntensity = 1.2f;
    private float bloomTimer = 0f;
    public float specialBloomDuration = 0.5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        volume = GetComponent<Volume>();

        // Get references to each effect
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
    }

    void Update()
    {
        HandleAberration();
        HandleBloom();
    }

    // ─── CHROMATIC ABERRATION ─────────────────────
    void HandleAberration()
    {
        if (aberrationTimer > 0)
        {
            aberrationTimer -= Time.unscaledDeltaTime;

            if (chromaticAberration != null)
                chromaticAberration.intensity.value =
                    Mathf.Lerp(0, damageAberrationIntensity,
                    aberrationTimer / damageAberrationDuration);

            if (aberrationTimer <= 0 &&
                chromaticAberration != null)
                chromaticAberration.intensity.value = 0f;
        }
    }

    // ─── BLOOM ───────────────────────────────────
    void HandleBloom()
    {
        if (bloomTimer > 0)
        {
            bloomTimer -= Time.unscaledDeltaTime;

            if (bloom != null)
                bloom.intensity.value = Mathf.Lerp(
                    normalBloomIntensity,
                    specialBloomIntensity,
                    bloomTimer / specialBloomDuration
                );

            if (bloomTimer <= 0 && bloom != null)
                bloom.intensity.value = normalBloomIntensity;
        }
    }

    // ─── PUBLIC TRIGGERS ─────────────────────────
    public void TriggerDamageEffect()
    {
        aberrationTimer = damageAberrationDuration;

        // Flash vignette red briefly
        if (vignette != null)
            StartCoroutine(VignetteFlash());
    }

    public void TriggerSpecialEffect()
    {
        bloomTimer = specialBloomDuration;
    }

    System.Collections.IEnumerator VignetteFlash()
    {
        if (vignette == null) yield break;

        vignette.color.value = Color.red;
        vignette.intensity.value = 0.6f;

        yield return new WaitForSecondsRealtime(0.15f);

        vignette.color.value = Color.black;
        vignette.intensity.value = 0.35f;
    }
}