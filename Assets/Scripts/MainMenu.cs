using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonPressed);
        settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        creditsButton.onClick.AddListener(OnCreditsButtonPressed);
        exitButton.onClick.AddListener(OnExitButtonPressed);
    }

    void OnPlayButtonPressed()
    {
        Debug.Log("Play button pressed");
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
}
