using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    private Movement _movement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        _movement = GetComponent<Movement>();
    }

    private void Update()
    {
        _movement.HorizontalInput = InputManager.Instance.Horizontal;
        _movement.VerticalInput = InputManager.Instance.Vertical;
    }

    public void EnableMovement() => _movement.Enable();
    public void DisableMovement() => _movement.Disable();
}
