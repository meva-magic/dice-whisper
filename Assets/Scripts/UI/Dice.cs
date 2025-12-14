using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private Image _diceSprite;
    [SerializeField] private Sprite[] _diceSprites;
    [SerializeField][Min(0f)] private float _animationSpriteChangeDelay = 0.25f;
    [SerializeField][Min(0f)] private float _animationRotationsPerSecond = 2f;

    private float _animationRotationSpeed;

    private int _value;
    public int Value
    {
        get => _value;

        set
        {
            if (value >= 1 && value <= 6)
            {
                _value = value;
                _diceSprite.sprite = _diceSprites[value - 1];
            }
            else
            {
                Debug.LogWarning("ѕопытка установить значение дайса за пределами 1-6");
            }
        }
    }

    private bool _isAnimating = false;

    private void Start()
    {
        bool hasNullSprites = false;

        foreach (var sprite in _diceSprites)
        {
            if (sprite == null)
            {
                hasNullSprites = true;
                break;
            }    
        }

        if (_diceSprites.Length != 6 || hasNullSprites)
            Debug.LogWarning("” дайса задано меньше 6 спрайтов");

        Value = 6;

        _animationRotationSpeed = _animationRotationsPerSecond * 360f;
    }

    public void StartAnimation()
    {
        _isAnimating = true;

        StopAllCoroutines();
        StartCoroutine(AnimateSprite());
        StartCoroutine(AnimateRotation());
    }

    public void StopAnimation()
    {
        _isAnimating = false;
    }

    private IEnumerator AnimateRotation()
    {
        float t = 0f;

        while (_isAnimating)
        {
            transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Repeat(_animationRotationSpeed * t, 360f));

            t += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(RotateToDefault());
    }

    private IEnumerator RotateToDefault()
    {
        float startingAngle = transform.localEulerAngles.z;

        float closestUprightAngle = startingAngle switch
        {
            < 90f => 90f,
            < 180f => 180f,
            < 270f => 270f,
            _ => 360f
        };

        float timeToClosestUprightAngle = (closestUprightAngle - startingAngle) / _animationRotationSpeed;

        float t = 0f;

        while (transform.localEulerAngles.z < closestUprightAngle)
        {
            transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(startingAngle, closestUprightAngle, t / timeToClosestUprightAngle));

            t += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new(0f, 0f, 0f);
    }

    private IEnumerator AnimateSprite()
    {
        while (_isAnimating)
        {
            Value = Random.Range(1, 7);
            yield return new WaitForSeconds(_animationSpriteChangeDelay);
        }
    }
}
