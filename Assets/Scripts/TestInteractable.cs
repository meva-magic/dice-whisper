using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TestInteractable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
        => _spriteRenderer = GetComponent<SpriteRenderer>();

    public void ChangeColor()
        => _spriteRenderer.color = Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.7f, saturationMax: 0.7f, valueMin: 0.95f, valueMax: 0.95f);
}
