using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseModal : Modal
{
    [SerializeField] private SettingsModal _settingsModal;

    public void Resume()
        => Deactivate();

    public void Settings()
        => _settingsModal.Activate();

    public void ToMenu()
    {
        Deactivate();
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

    protected override void OnActivate()
        => Time.timeScale = 0f;

    protected override void OnDeactivate()
    {
        Time.timeScale = 1f;

        if (_settingsModal.IsActive)
            _settingsModal.Deactivate();
    }
}
