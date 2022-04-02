using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TrackCheckpoints : NetworkBehaviour
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
        var index = checkpoints.IndexOf(checkpoint);
        if (index > currentCheckpointIndex)
        {
            // store new checkpoint in save data
            currentCheckpointIndex = index;
            GameManager.instance.UpdateCheckpoint(index);
        }
    }
}
