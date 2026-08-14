using UnityEngine;

// Fixes review comment #3: OnDrawGizmosSelected only shows in the Scene
// view while editing. This draws an actual visible circle using a
// LineRenderer, so it works in Play Mode and in builds - useful both for
// the placement preview ("ghost") and on armed TNT in the scene.
[RequireComponent(typeof(LineRenderer))]
public class ExplosionRadiusIndicator : MonoBehaviour
{
    [SerializeField] private int segments = 48;
    [SerializeField] private Color radiusColor = new Color(1f, 0.3f, 0.2f, 0.6f);

    private Vector2[] unitCircle; //cache sin/cos directions

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = false;
        line.positionCount = segments;
        line.widthMultiplier = 0.05f;
        line.startColor = radiusColor;
        line.endColor = radiusColor;
        line.material = new Material(Shader.Find("Sprites/Default"));

        BuildUnitCircle(); // new 
    }

    private void BuildUnitCircle()
    {
        unitCircle = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            unitCircle[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    public void SetRadius(float radius)
    {
        //for (int i = 0; i < segments; i++)
        //{
        //    float angle = 2 * Mathf.PI * i / segments;
        //    float x = Mathf.Cos(angle) * radius;
        //    float y = Mathf.Sin(angle) * radius;
        //    line.SetPosition(i, new Vector3(x, y, 0f));
        //}
        for (int i = 0; i < segments; i++)
        {
            Vector2 dir = unitCircle[i];
            line.SetPosition(i, new Vector3(dir.x * radius, dir.y * radius, 0f));
        }
    }

    public void SetVisible(bool visible)
    {
        line.enabled = visible;
    }
}
