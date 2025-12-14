using System;
using UnityEngine;

public class DialogStarter : MonoBehaviour
{
    [SerializeField] private string _dialogFileName;

    public void StartDialog()
    {
        Dialog dialog = ReadDialogFromJson();
        DialogWalker.Instance.StartDialog(dialog);
    }

    private Dialog ReadDialogFromJson()
    {
        if (string.IsNullOrEmpty(_dialogFileName))
        {
            Debug.LogWarning($"На объекте {gameObject.name} находится компонент DialogStarter с пустым названием диалога.");
            return null;
        }

        TextAsset dialogFile = Resources.Load<TextAsset>($"Dialogs/{_dialogFileName}");

        if (dialogFile == null)
        {
            Debug.LogError($"Диалог \"{_dialogFileName}\" не был найден в папке с ресурсами");
            return null;
        }

        Dialog loadedDialog = null;

        try
        {
            loadedDialog = JsonUtility.FromJson<Dialog>(dialogFile.text);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Произошла ошибка при десериализации диалога \"{_dialogFileName}\": {ex.Message}");
        }

        return loadedDialog;
    }
}
