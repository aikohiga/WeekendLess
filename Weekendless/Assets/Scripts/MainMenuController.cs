using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Главная панель")]
    public GameObject mainPanel;
    public Button startButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Панель настроек")]
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Button backButton;

    [Header("Настройки игры")]
    public string gameSceneName = "GameScene";

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(ShowSettings);
        exitButton.onClick.AddListener(ExitGame);

        backButton.onClick.AddListener(ShowMainMenu);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        LoadSettings();

        ShowMainMenu();
    }

    public void StartGame()
    {
        Debug.Log("Загрузка игры...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    void LoadSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        Screen.fullScreen = savedFullscreen;
    }

    public void ExitGame()
    {
        Debug.Log("Выход из игры");

        PlayerPrefs.Save();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}