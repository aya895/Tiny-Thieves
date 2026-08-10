using UnityEngine;

public class AntStateController : MonoBehaviour
{
    public AntState CurrentState { get; private set; } = AntState.Moving;

    public void SetState(AntState newState)
    {
        CurrentState = newState;
    }
}