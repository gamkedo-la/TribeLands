using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelEndpoint : MonoBehaviour
{
    public string NextScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    // public void LoadNextScene(InputAction.CallbackContext ctx)
    public void LoadNextScene()
    {
        SceneManager.LoadScene(NextScene);
    }
    
    void OnTriggerEnter(Collider other)
    {
        var avatar = other.GetComponent<NetworkAvatar>();
        if (avatar != null) {
            LoadNextScene();
            Debug.Log($"triggered by {other.name}, load next level");
        }
    }

}
