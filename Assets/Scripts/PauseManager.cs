using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [SerializeField] private PauseModal _pauseModal;

    private bool _isEnabled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    private void Update()
    {
        if (InputManager.Instance.Pause && _isEnabled)
            _pauseModal.Toggle();
    }

    public void EnablePause() => _isEnabled = true;
    public void DisablePause() => _isEnabled = false;
}
