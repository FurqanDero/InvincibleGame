using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public RectTransform fillRect;
    public float maxWidth = 600f;

    public void UpdateHealth(float current, float max)
    {
        float ratio = current / max;
        fillRect.sizeDelta = new Vector2(
            maxWidth * ratio,
            fillRect.sizeDelta.y
        );

        // Color shifts red as health drops
        Image fill = fillRect.GetComponent<Image>();
        if (fill != null)
            fill.color = Color.Lerp(
                Color.red,
                new Color(1f, 0.6f, 0f),
                ratio
            );
    }
}