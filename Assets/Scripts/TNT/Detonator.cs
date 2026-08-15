using UnityEngine;

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
