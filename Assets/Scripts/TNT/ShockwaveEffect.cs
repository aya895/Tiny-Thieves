using System.Collections;
using UnityEngine;

// SINGLE RESPONSIBILITY: purely a visual ring that expands and fades.
// Knows nothing about TNT, explosions, or damage - TNTVisual tells it
// what radius range and duration to animate.
[RequireComponent(typeof(LineRenderer))]
public class ShockwaveEffect : MonoBehaviour
{
    [SerializeField] private int segments = 48;
    [SerializeField] private Color ringColor = new Color(1f, 0.6f, 0.1f, 0.85f);

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = false;
        line.positionCount = segments;
        line.widthMultiplier = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = ringColor;
        line.endColor = ringColor;
    }

    public void Play(float startRadius, float endRadius, float duration)
    {
        StartCoroutine(Expand(startRadius, endRadius, duration));
    }

    private IEnumerator Expand(float startRadius, float endRadius, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            SetRadius(Mathf.Lerp(startRadius, endRadius, t));

            Color c = ringColor;
            c.a = Mathf.Lerp(ringColor.a, 0f, t); // fade out as it expands
            line.startColor = c;
            line.endColor = c;

            yield return null;
        }

        Destroy(gameObject);
    }

    private void SetRadius(float radius)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
