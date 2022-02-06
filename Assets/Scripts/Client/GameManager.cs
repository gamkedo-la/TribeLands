using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages events within the local / client environment
// Anything that doesn't need to be sent over the network
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void TogglePauseMenu(){
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
