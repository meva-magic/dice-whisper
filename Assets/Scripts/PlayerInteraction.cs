using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField][Min(0f)] private float _interactionBoxWidth;
    [SerializeField][Min(0f)] private float _interactionBoxHeight;

    [SerializeField] private LayerMask _interactablesLayerMask;

    private Vector2 InteractionBoxSize => new(_interactionBoxWidth, _interactionBoxHeight);

    private readonly List<Interactable> _interactables = new();

    private void Update()
    {
        foreach (Interactable interactable in _interactables)
            interactable.HideIndicator();

        _interactables.Clear();

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, InteractionBoxSize, 0f, _interactablesLayerMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<Interactable>(out var interactable))
            {
                _interactables.Add(interactable);
                interactable.ShowIndicator();
            }
        }

        if (InputManager.Instance.Interact)
        {
            foreach (Interactable interactable in _interactables)
                interactable.Interact();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, InteractionBoxSize);
    }
}
