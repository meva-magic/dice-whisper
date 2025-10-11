using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Movement : MonoBehaviour
{
    [SerializeField] private bool _spriteIsFlippedByDefault = false;
    public bool SpriteIsFlippedByDefault => _spriteIsFlippedByDefault;

    [SerializeField][Min(0f)] private float _moveSpeed = 5f;

    public float HorizontalInput { get; set; } = 0f;
    public float VerticalInput { get; set; } = 0f;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private bool _canMove = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        TryGetComponent(out _animator);
    }

    private void Start()
    {
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (_animator != null)
            _animator.SetBool("Move", Mathf.Abs(HorizontalInput) > 0f || Mathf.Abs(VerticalInput) > 0f);
    }

    private void FixedUpdate()
    {
        if (!_canMove)
            return;

        Walk();
    }

    private void Walk()
    {
        Vector2 moveDir = new Vector2(HorizontalInput, VerticalInput).normalized;
        _rigidbody.linearVelocity = moveDir * _moveSpeed;

        if (HorizontalInput > 0f)
            _spriteRenderer.flipX = _spriteIsFlippedByDefault;
        else if (HorizontalInput < 0f)
            _spriteRenderer.flipX = !_spriteIsFlippedByDefault;
    }

    public void Enable()
    {
        _canMove = true;
    }

    public void Disable()
    {
        _canMove = false;
        _rigidbody.linearVelocity = Vector2.zero;
    }
}
