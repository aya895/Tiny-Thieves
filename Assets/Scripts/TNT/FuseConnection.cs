using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FuseConnection : MonoBehaviour
{
    [SerializeField]
    private Color fuseColor =
        new Color(
            0.55f,
            0.35f,
            0.15f
        );

    private LineRenderer line;
    private TNTLogic from;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.positionCount = 2;
        line.widthMultiplier = 0.08f;

        line.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        line.startColor = fuseColor;
        line.endColor = fuseColor;
    }

    public float Setup(
        TNTLogic fromTNT,
        Vector3 to)
    {
        from = fromTNT;

        line.SetPosition(
            0,
            fromTNT.transform.position
        );

        line.SetPosition(
            1,
            to
        );

        from.OnExplode +=
            HandleFromExploded;

        return Vector3.Distance(
            fromTNT.transform.position,
            to
        );
    }

    private void HandleFromExploded()
    {
        if (from != null)
        {
            from.OnExplode -=
                HandleFromExploded;
        }

        Destroy(
            gameObject,
            0.2f
        );
    }

    private void OnDestroy()
    {
        if (from != null)
        {
            from.OnExplode -=
                HandleFromExploded;
        }
    }
}