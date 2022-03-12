using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Top-level menu")]
    [SerializeField] private GameObject mainMenuOptionsPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Selectable mainMenuFirstSelected;
    private GameObject mainMenuPreviousSelection;

    [Space]
    [Header("Play")]
    [SerializeField] private GameObject playPanel;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField hostAddressInput;
    [SerializeField] private Selectable playPanelFirstSelected;
    [SerializeField] private Button playPanelBackButton;
    
    [Space]
    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Selectable settingsPanelFirstSelected;
    [SerializeField] private Button settingsPanelBackButton;
    
    [Space]
    [Header("Credits")]
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Selectable creditsPanelFirstSelected;
    [SerializeField] private Button creditsPanelBackButton;


    private GameObject activePanel;
    private NetworkPlayerInput playerInput;
    private InputDevice activeDevice;

    private void Awake()
    {
        playerInput = new NetworkPlayerInput();
    }

    void Start()
    {
        // In case we're kicked back here from within game, make sure cursor is not locked.
        Cursor.lockState = CursorLockMode.None;
        
        playButton.onClick.AddListener(() => SwitchPanel(playPanel, playPanelFirstSelected.gameObject));
        settingsButton.onClick.AddListener(() => SwitchPanel(settingsPanel, settingsPanelFirstSelected.gameObject));
        creditsButton.onClick.AddListener(() => SwitchPanel(creditsPanel, creditsPanelFirstSelected.gameObject));
        exitButton.onClick.AddListener(OnExitButtonPressed);
        
        hostButton.onClick.AddListener(OnHostButtonPressed);
        joinButton.onClick.AddListener(OnJoinButtonPressed);
        hostAddressInput.onValueChanged.AddListener(OnHostValueChanged);
        
        playPanelBackButton.onClick.AddListener(BackButtonPressed);
        settingsPanelBackButton.onClick.AddListener(BackButtonPressed);
        creditsPanelBackButton.onClick.AddListener(BackButtonPressed);
        
        // Apply default state.
        mainMenuOptionsPanel.SetActive(true);
        playPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
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

    void SwitchPanel(GameObject panel, GameObject firstSelected)
    {
        activePanel = panel;
        mainMenuOptionsPanel.SetActive(false);
        panel.SetActive(true);
        mainMenuPreviousSelection = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    void BackButtonPressed()
    {
        activePanel.SetActive(false);
        mainMenuOptionsPanel.SetActive(true);
        RestoreMainMenuSelection();
    }
    void RestoreMainMenuSelection()
    {
        EventSystem.current.SetSelectedGameObject(
            mainMenuPreviousSelection == null ?
                mainMenuFirstSelected.gameObject :
                mainMenuPreviousSelection);
    }
}
