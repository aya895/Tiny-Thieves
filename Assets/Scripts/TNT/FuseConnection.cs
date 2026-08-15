using UnityEngine;

// SINGLE RESPONSIBILITY: only draws the fuse line between two TNTs and
// reports the distance between them, which TNTPlacementController then
// hands to TNTLogic to compute chain delay (distance / burn speed).
[RequireComponent(typeof(LineRenderer))]
public class FuseConnection : MonoBehaviour
{
    [SerializeField] private Color fuseColor = new Color(0.55f, 0.35f, 0.15f); // brown rope/fuse color

    private LineRenderer line;
    private TNTLogic from;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.widthMultiplier = 0.08f;
        line.material = new Material(Shader.Find("Sprites/Default")); // fixes the default-magenta look
        line.startColor = fuseColor;
        line.endColor = fuseColor;
    }

    // Takes the FROM TNT itself (not just its position) so this fuse can
    // subscribe to its OnExplode event and clean itself up at the right
    // moment - no reliance on parent/child transform hierarchy.
    public float Setup(TNTLogic fromTNT, Vector3 to)
    {
        from = fromTNT;
        line.SetPosition(0, fromTNT.transform.position);
        line.SetPosition(1, to);

        from.OnExplode += HandleFromExploded;

        return Vector3.Distance(fromTNT.transform.position, to);
    }

    private void HandleFromExploded(Vector2 position, float radius, float damage)
    {
        from.OnExplode -= HandleFromExploded;
        Destroy(gameObject, 0.2f); // small delay so the burn-away visual can finish
    }
}