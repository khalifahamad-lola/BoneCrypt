using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Checkpoint currentActiveCheckpoint;

    public void ActivateCheckpoint(Checkpoint newCheckpoint)
    {
        if (newCheckpoint == null)
            return;

        if (currentActiveCheckpoint != null && currentActiveCheckpoint != newCheckpoint)
        {
            currentActiveCheckpoint.SetActiveState(false);
        }

        currentActiveCheckpoint = newCheckpoint;
        currentActiveCheckpoint.SetActiveState(true);
    }
}