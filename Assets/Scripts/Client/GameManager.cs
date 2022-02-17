using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages events within the local / client environment
// Anything that doesn't need to be sent over the network
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject buttonPanelMain;
    [SerializeField] GameObject optionsMenu;
    private GameObject localPlayer;
    private GameObject currentMenu;

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
        localPlayer.GetComponent<NetworkPlayer>()?.ResumeGame();
    }
    public void CloseAllMenus(){
        GameObject[] menus = gameObject.GetComponentsInChildren<GameObject>();
        foreach (var menu in menus){
            menu.SetActive(false);
        }
    }
    public void SetLocalPlayer(GameObject playerObj){
        localPlayer = playerObj;
    }
    private void Awake(){
        currentMenu = pauseMenu;
    }
    void Start(){
        
    }
    void Update(){
        
    }
}
