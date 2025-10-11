using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothTime = 0.15f;

    [Header("World Borders")]

    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;

    private Camera _camera;

    private Vector2 _velocity = Vector2.zero;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (_target == null)
            return;

        Vector2 smoothedPosition = Vector2.SmoothDamp(transform.position, _target.position, ref _velocity, _smoothTime);

        float verticalExtent = _camera.orthographicSize;
        float horizontalExtent = verticalExtent * _camera.aspect;

        float clampedX = Mathf.Clamp(smoothedPosition.x, _minX + horizontalExtent, _maxX - horizontalExtent);
        float clampedY = Mathf.Clamp(smoothedPosition.y, _minY + verticalExtent, _maxY - verticalExtent);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(new Vector3(_minX, _minY, 0f), new Vector3(_maxX, _minY, 0f));
        Gizmos.DrawLine(new Vector3(_maxX, _minY, 0f), new Vector3(_maxX, _maxY, 0f));
        Gizmos.DrawLine(new Vector3(_maxX, _maxY, 0f), new Vector3(_minX, _maxY, 0f));
        Gizmos.DrawLine(new Vector3(_minX, _maxY, 0f), new Vector3(_minX, _minY, 0f));
    }
}
