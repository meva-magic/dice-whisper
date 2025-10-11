using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject _indicator;

    [SerializeField] private Component _componentToActivate;
    [SerializeField] private string _methodName;

    private void Start()
        => _indicator.SetActive(false);

    public void Interact()
        => _componentToActivate.SendMessage(_methodName, SendMessageOptions.RequireReceiver);

    public void ShowIndicator()
    {
        if (_indicator != null)
            _indicator.SetActive(true);
    }

    public void HideIndicator()
    {
        if (_indicator != null)
            _indicator.SetActive(false);
    }
}
