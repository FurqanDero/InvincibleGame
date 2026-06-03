using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float parallaxMultiplier = 0.5f;
    public bool infiniteScrolling = true;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private SpriteRenderer sr;

    void Start()
    {    
        // Find camera manually in case Camera.main fails
        cameraTransform = GameObject
            .FindWithTag("MainCamera")?.transform;

        if (cameraTransform == null)
            cameraTransform = Camera.main?.transform;

        if (cameraTransform == null)
        {
            Debug.LogError("ParallaxBackground: " +
                "No camera found on " + gameObject.name);
            enabled = false;
            return;
        }

        lastCameraPosition = cameraTransform.position;
        sr = GetComponent<SpriteRenderer>();

        // Get texture width for looping
        if (sr != null && sr.sprite != null)
        {
            Texture2D texture = sr.sprite.texture;
            textureUnitSizeX = texture.width /
                               sr.sprite.pixelsPerUnit;
        }

        Debug.Log(gameObject.name + " camera found: " +
          (cameraTransform != null));
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position -
                                lastCameraPosition;

        // Move at fraction of camera speed
        transform.position += new Vector3(
            deltaMovement.x * parallaxMultiplier,
            0f, // don't move vertically
            0f
        );

        lastCameraPosition = cameraTransform.position;

        // Infinite horizontal scrolling
        if (infiniteScrolling && textureUnitSizeX > 0)
        {
            float distanceFromCamera =
                cameraTransform.position.x -
                transform.position.x;

            if (Mathf.Abs(distanceFromCamera) >=
                textureUnitSizeX)
            {
                float offsetPositionX =
                    distanceFromCamera % textureUnitSizeX;
                transform.position = new Vector3(
                    cameraTransform.position.x +
                    offsetPositionX,
                    transform.position.y,
                    transform.position.z
                );
            }
        }
    }
}