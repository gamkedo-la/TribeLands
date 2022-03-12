using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages events within the local / client environment
// Anything that doesn't need to be sent over the network
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject buttonPanelMain;
    [SerializeField] GameObject optionsMenu;
    private NetworkPlayer localPlayerController;
    private GameObject currentMenu;
    private bool gameIsOn = false;
    public void HandleResume(){
        CheckForLocalPlayer();
        localPlayerController.Resume();
    }
    public void ToOptionsMenu(){
        pauseMenu.SetActive(true);
        buttonPanelMain.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void OpenPauseMenu(){        
        pauseMenu.SetActive(true);
        buttonPanelMain.SetActive(true);
        optionsMenu.SetActive(true);
    }
    public void ClosePauseMenu(){
        pauseMenu.SetActive(false);
        buttonPanelMain.SetActive(false);
        optionsMenu.SetActive(false);
    }
    public void CheckForLocalPlayer(){
        localPlayerController = FindObjectOfType<NetworkPlayer>();
    }
    public bool GameIsOn(){
        return gameIsOn;
    }
    public void SetGameIsOn(bool g){
        gameIsOn = g;
    }
    private void Awake(){
        currentMenu = pauseMenu;
    }
    void Start(){
        
    }
    void Update(){
        
    }
}
