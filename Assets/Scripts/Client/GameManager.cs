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
    private GameObject currentMenu;

    public void ToggleOptionsMenu(){
        pauseMenu.SetActive(!optionsMenu.activeSelf);
    }
    public void OpenPauseMenu(){
        pauseMenu.SetActive(true);
        buttonPanelMain.SetActive(true);
        currentMenu = buttonPanelMain;
    }
    public void CloseAllMenus(){
        GameObject[] menus = gameObject.GetComponentsInChildren<GameObject>();
        foreach (var menu in menus){
            menu.SetActive(false);
        }
        currentMenu = pauseMenu;
    }
    public GameObject GetActiveMenu(){
        return currentMenu;
    }
    private void Awake(){
        currentMenu = pauseMenu;
    }
    void Start(){
        
    }
    void Update(){
        
    }
}
