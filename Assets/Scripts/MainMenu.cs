using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject playPanel;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonPressed);
        settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        creditsButton.onClick.AddListener(OnCreditsButtonPressed);
        exitButton.onClick.AddListener(OnExitButtonPressed);
        
        hostButton.onClick.AddListener(OnHostButtonPressed);
        joinButton.onClick.AddListener(OnJoinButtonPressed);
        
        playPanel.SetActive(false);
    }

    void OnPlayButtonPressed()
    {
        Debug.Log("Play button pressed");
        playPanel.SetActive(true);
    }

    void OnSettingsButtonPressed()
    {
        Debug.Log("Settings button pressed");
    }

    void OnCreditsButtonPressed()
    {
        Debug.Log("Credits button pressed");
    }

    void OnExitButtonPressed()
    {
#if UNITY_EDITOR
        Debug.Log("Exit button pressed");
#else
        Application.Quit();
#endif
    }

    void OnHostButtonPressed()
    {
        GroupNetworkManager.singleton.StartHost();
        GroupNetworkManager.singleton.ServerChangeScene("Dungeon");
    }

    void OnJoinButtonPressed()
    {
        var ipAddress = "localhost";
        GroupNetworkManager.singleton.networkAddress = ipAddress;
        GroupNetworkManager.singleton.StartClient();
        // Debug.Log($"Join game at {ipAddress}");
        // 1. Check that an IP address was provided
        // 2. Join game at that address
    }
}
