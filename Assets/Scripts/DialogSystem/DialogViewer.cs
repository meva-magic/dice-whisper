using System.Collections;
using TMPro;
using UnityEngine;

public class DialogViewer : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeTime;

    [Space]

    [SerializeField] private TMP_Text _characterName;
    [SerializeField] private TMP_Text _mainText;

    public void Open()
    {
        StopCoroutine(FadeOut());
        StartCoroutine(FadeIn());
    }

    public void Close()
    {
        StopCoroutine(FadeIn());
        StartCoroutine(FadeOut());
    }

    public void ShowItem(DialogItem item)
    {
        _characterName.text = item.CharacterName;
        _mainText.text = item.Text;
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;

        while (_canvasGroup.alpha < 1f)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / _fadeTime);

            time += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        while (_canvasGroup.alpha > 0f)
        {
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / _fadeTime);

            time += Time.deltaTime;
            yield return null;
        }
    }
}
