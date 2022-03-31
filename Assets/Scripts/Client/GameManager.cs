using UnityEngine;

// Manages events within the local / client environment
// Anything that doesn't need to be sent over the network
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject buttonPanelMain;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] private LevelData levelData;
    
    private NetworkPlayer localPlayerController;
    private GameObject currentMenu;
    public void HandleResume(){
        CheckForLocalPlayer();
        localPlayerController.Resume();
    }
    public void ToOptionsMenu(){
        pauseMenu.SetActive(true);
        buttonPanelMain.SetActive(false);
        optionsMenu.SetActive(true);
        currentMenu = optionsMenu;
    }
    public void OpenPauseMenu(){        
        pauseMenu.SetActive(true);
        buttonPanelMain.SetActive(true);
        optionsMenu.SetActive(false);
    }
    public void ClosePauseMenu(){
        pauseMenu.SetActive(false);
        buttonPanelMain.SetActive(false);
        optionsMenu.SetActive(false);
    }
    public void CheckForLocalPlayer(){
        localPlayerController = FindObjectOfType<NetworkPlayer>();
    }

    public void ToggleMute()
    {
        Debug.Log("anything from ToggleMute()");
    }
    private void Awake(){
        currentMenu = pauseMenu;
    }
    
    void Start()
    {
        if (levelData != null)
            AudioManager.instance.SetBackgroundMusic(levelData.BackgroundMusic);
    }
    
    void Update(){
        
    }
}
