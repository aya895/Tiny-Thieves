using UnityEngine;

// SINGLE RESPONSIBILITY: fires the first TNT in the chain. Hook
// DetonateChain() to a UI Button's OnClick.
public class Detonator : MonoBehaviour
{
    [SerializeField] private TNTPlacementController placementController;

    public void DetonateChain()
    {
        TNTLogic start = placementController.GetChainStart();
        if (start != null)
        {
            start.Ignite();
        }
    }
}
