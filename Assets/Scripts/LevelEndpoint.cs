using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndpoint : MonoBehaviour
{
    public string NextScene;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(NextScene);
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
