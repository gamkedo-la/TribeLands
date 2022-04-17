using Mirror;
using UnityEngine;
// Manages events within the local / client environment
// Anything that doesn't need to be sent over the network
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject buttonPanelMain;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] private LevelData levelData;
    [SerializeField] private PlayerData playerData;

    bool muted = false;

    public PlayerData PlayerData
    {
        get => playerData;
    }

    private NetworkPlayer localPlayerController;
    private GameObject currentMenu;
    public void HandleResume(){
        CheckForLocalPlayer();
        currentMenu = pauseMenu;
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
        currentMenu = pauseMenu;
    }
    public void ClosePauseMenu(){
        pauseMenu.SetActive(false);
        buttonPanelMain.SetActive(false);
        optionsMenu.SetActive(false);
        currentMenu = pauseMenu;
    }

    public void Disconnect()
    {
        var gnm = GroupNetworkManager.singleton;
        if (NetworkClient.isHostClient)
        {
            gnm.StopHost();
        }
        else
        {
            gnm.StopClient();
        }
        ClosePauseMenu();
    }

    public void UpdateCheckpoint(int checkpointIndex)
    {
        playerData.checkpointIndex = checkpointIndex;
        playerData.scene = levelData.SceneName;
        WriteSave();
    }

    private void WriteSave()
    {
        Debug.Log("Writing data to save file");
        SaveSystem.SaveGame(playerData);
    }
    
    public void CheckForLocalPlayer(){
        localPlayerController = FindObjectOfType<NetworkPlayer>();
    }

    public void ToggleMute()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        AudioListener mainCamerasAudioListener = mainCamera.GetComponent<AudioListener>();

        if (!muted)
        {
            mainCamerasAudioListener.enabled = false;
            muted = true;           
        }
        else
        {
            mainCamerasAudioListener.enabled = true;
            muted = false;  
        }
    }

    private void Awake(){
        currentMenu = pauseMenu;
        
        if (instance != null && instance != this)
        {
            if (levelData != null)
            {
                // Assign our LevelData to the main instance before getting destroyed.
                instance.levelData = levelData;
            }
            
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        // DontDestroyOnLoad doesn't work for nested objects.
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (levelData != null)
            AudioManager.instance.SetBackgroundMusic(levelData.BackgroundMusic);

        if (playerData != null)
        {
            var saveData = SaveSystem.LoadGame();
            playerData.Load(saveData);
        }
    }
    void Update(){
        
    }
}
