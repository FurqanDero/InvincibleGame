using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        if (cinemachineCamera != null)
            noise = cinemachineCamera
                .GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity, float duration)
    {
        if (noise != null)
        {
            noise.AmplitudeGain = intensity;
            shakeTimer = duration;
        }
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                if (noise != null)
                    noise.AmplitudeGain = 0f;
            }
        }
    }
}