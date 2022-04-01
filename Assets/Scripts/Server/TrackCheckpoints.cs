using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    private List<Checkpoint> checkpoints;
    private int currentCheckpointIndex = -1;

    private void Awake()
    {
        Transform checkpointsParent = transform.Find("Checkpoints");

        checkpoints = new List<Checkpoint>();
        foreach (Transform checkpointTransform in checkpointsParent)
        {
            var checkpoint = checkpointTransform.GetComponent<Checkpoint>();
            checkpoint.SetTracker(this);
            checkpoints.Add(checkpoint);
        }
    }

    public void PlayerReachedCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoints.IndexOf(checkpoint) > currentCheckpointIndex)
        {
            // store new checkpoint in save data
            GameManager.instance.UpdateCheckpoint(currentCheckpointIndex);
        }
    }
}
