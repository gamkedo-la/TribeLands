using Mirror;
using UnityEngine;

public class LevelEndpoint : MonoBehaviour
{
    public string NextScene;

    [Server]
    public void LoadNextScene()
    {
        GroupNetworkManager.singleton.ServerChangeScene(NextScene);
    }
    
    void OnTriggerEnter(Collider other)
    {
        var avatar = other.GetComponent<NetworkAvatar>();
        if (avatar != null)
        {
            LoadNextScene();
        }
    }

}
