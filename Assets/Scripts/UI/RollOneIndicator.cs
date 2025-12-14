using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RollOneIndicator : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _color;
    [SerializeField][Min(0f)] private float _blinkDelay = 0.25f;

    private Color _colorWithAlpha;

    private bool _isBlinking = false;

    private void Start()
    {
        _colorWithAlpha = new Color(_color.r, _color.g, _color.b, 0f);
    }

    public void Enable()
    {
        _isBlinking = true;
        StartCoroutine(Blink());
    }

    public void Disable()
    {
        _isBlinking = false;
    }

    private IEnumerator Blink()
    {
        while (_isBlinking)
        {
            _image.color = _color;
            yield return new WaitForSeconds(_blinkDelay);
            _image.color = _colorWithAlpha;
            yield return new WaitForSeconds(_blinkDelay);
        }
    }
}
