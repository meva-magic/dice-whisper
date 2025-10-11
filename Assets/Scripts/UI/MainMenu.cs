using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private const string HasProgressKey = "has_progress";

    private readonly Color EnabledButtonTextColor = new Color32(0x00, 0x00, 0x00, 0xFF);
    private readonly Color DisabledButtonTextColor = new Color32(0x7F, 0x7F, 0x7F, 0xFF);

    [SerializeField] private Button _continueButton;
    [SerializeField] private TMP_Text _continueButtonText;
    [SerializeField] private SettingsModal _settingsModal;

    private void Start()
    {
        bool firstLaunch = !PlayerPrefs.HasKey(HasProgressKey);

        if (firstLaunch)
        {
            _continueButton.interactable = false;
            _continueButtonText.color = DisabledButtonTextColor;
        }
        else
        {
            _continueButton.interactable = true;
            _continueButtonText.color = EnabledButtonTextColor;
        }
    }

    public void Continue()
        => LoadGame();

    public void NewGame()
    {
        PlayerPrefs.SetInt(HasProgressKey, 1);
        PlayerPrefs.Save();

        LoadGame();
    }

    public void Settings()
        => _settingsModal.Activate();

    public void Quit()
        => Application.Quit();

    private void LoadGame()
        => SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
}
