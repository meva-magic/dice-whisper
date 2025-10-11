using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseModal _pauseModal;

    private void Update()
    {
        if (InputManager.Instance.Pause)
            _pauseModal.Toggle();
    }
}
