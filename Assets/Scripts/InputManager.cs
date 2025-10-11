using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] private float _smoothInputSpeed = 0.1f;
    [SerializeField] private float _smoothInputDeadZone = 0.01f;

    private PlayerInput _playerInput;

    private InputAction _move;
    private InputAction _interact;
    private InputAction _pause;

    public float Horizontal { get; private set; } = 0f;
    public float Vertical { get; private set; } = 0f;
    public bool Interact => _interact.WasPressedThisFrame();
    public bool Pause => _pause.WasPressedThisFrame();

    private float _currentHorizontalInput = 0f;
    private float _currentVerticalInput = 0f;

    private float _currentSmoothHorizontalInputVelocity;
    private float _currentSmoothVerticalInputVelocity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        _playerInput = FindFirstObjectByType<PlayerInput>();

        _move = _playerInput.actions["Move"];
        _interact = _playerInput.actions["Interact"];
        _pause = _playerInput.actions["Pause"];
    }

    private void Update()
    {
        Vector2 move = _move.ReadValue<Vector2>();

        float horizontal = move.x;
        float vertical = move.y;

        _currentHorizontalInput = Mathf.SmoothDamp(_currentHorizontalInput, horizontal, ref _currentSmoothHorizontalInputVelocity, _smoothInputSpeed);
        _currentVerticalInput = Mathf.SmoothDamp(_currentVerticalInput, vertical, ref _currentSmoothVerticalInputVelocity, _smoothInputSpeed);

        if (Mathf.Abs(_currentHorizontalInput) < _smoothInputDeadZone)
            _currentHorizontalInput = 0f;

        if (Mathf.Abs(_currentVerticalInput) < _smoothInputDeadZone)
            _currentVerticalInput = 0f;

        Horizontal = _currentHorizontalInput;
        Vertical = _currentVerticalInput;
    }
}
