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
    private GameObject localPlayer;
    private GameObject currentMenu;
    private bool gameIsOn = false;
    private NetworkPlayerInput playerInput;
    public void ToOptionsMenu(){
        pauseMenu.SetActive(true);
        buttonPanelMain.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void OpenPauseMenu(){
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        buttonPanelMain.SetActive(true);
        optionsMenu.SetActive(true);
        playerInput.UI.Enable();
        playerInput.Player.Disable();
    }
    public void ClosePauseMenu(){
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        buttonPanelMain.SetActive(false);
        optionsMenu.SetActive(false);
        playerInput.UI.Disable();
        playerInput.Player.Enable();
    }
    public void SetLocalPlayer(GameObject playerObj){
        localPlayer = playerObj;
    }
    public bool GameIsOn(){
        return gameIsOn;
    }
    public void SetGameIsOn(bool g){
        gameIsOn = g;
    }
    private void Awake(){
        currentMenu = pauseMenu;
        playerInput = new NetworkPlayerInput();
    }
    void Start(){
        
    }
    void Update(){
        
    }
}
