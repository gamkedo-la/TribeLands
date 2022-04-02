using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private TrackCheckpoints tracker;

    public void SetTracker(TrackCheckpoints trackCheckpoints)
    {
        tracker = trackCheckpoints;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (tracker == null) return;
        
        tracker.PlayerReachedCheckpoint(this);
    }
}
