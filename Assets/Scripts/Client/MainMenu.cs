using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private TMP_InputField hostAddressInput;
    [SerializeField] private Button playPanelBackButton;

    void Start()
    {
        // In case we're kicked back here from within game, make sure cursor is not locked.
        Cursor.lockState = CursorLockMode.None;
        
        playButton.onClick.AddListener(OnPlayButtonPressed);
        settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        creditsButton.onClick.AddListener(OnCreditsButtonPressed);
        exitButton.onClick.AddListener(OnExitButtonPressed);
        
        hostButton.onClick.AddListener(OnHostButtonPressed);
        joinButton.onClick.AddListener(OnJoinButtonPressed);
        hostAddressInput.onValueChanged.AddListener(OnHostValueChanged);
        
        playPanelBackButton.onClick.AddListener((() => playPanel.SetActive(false)));
        
        playPanel.SetActive(false);
    }

    void OnPlayButtonPressed()
    {
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
    }

    void OnHostValueChanged(string hostAddress)
    {
        GroupNetworkManager.singleton.networkAddress = hostAddress;
    }

    void OnJoinButtonPressed()
    {
        GroupNetworkManager.singleton.StartClient();
        // TODO: Open another panel showing progress and/or errors from connecting to host.
    }
}
