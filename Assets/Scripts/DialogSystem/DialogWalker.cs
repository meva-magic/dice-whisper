using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogViewer))]
public class DialogWalker : MonoBehaviour
{
    public static DialogWalker Instance;

    private DialogViewer _viewer;

    private Dialog _currentDialog;
    private int _currentItemIndex;

    private readonly List<DialogItem> _dialogItemsWithTriggeredDiceGame = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;

        _viewer = GetComponent<DialogViewer>();
    }

    private void Update()
    {
        if (InputManager.Instance.Interact)
            GoToNextItem();
    }

    public void StartDialog(Dialog dialog)
    {
        _currentDialog = dialog;
        _currentItemIndex = 0;

        if (_currentDialog == null || _currentDialog.Items.Count == 0)
        {
            Exit();
            return;
        }

        PlayerMovement.Instance.DisableMovement();
        PlayerInteraction.Instance.DisableInteraction();

        _viewer.ShowItem(_currentDialog.Items[_currentItemIndex]);
        _viewer.Open();
    }

    public void GoToNextItem()
    {
        if (_currentDialog == null)
            return;

        if (_currentItemIndex >= _currentDialog.Items.Count)
            return;

        DialogItem currentItem = _currentDialog.Items[_currentItemIndex];

        if (currentItem.StartsDiceGame)
        {
            if (_dialogItemsWithTriggeredDiceGame.Contains(currentItem))
            {
                DiceGameManager.Instance.StoppedGame.RemoveListener(GoToNextItem);
                PlayerInteraction.Instance.DisableInteraction();
            }
            else
            {
                EnemyCharacter enemyCharacter = DialogHelper.Instance.GetEnemyCharacterForDialog(_currentDialog.Name);
                DiceGameManager.Instance.StoppedGame.AddListener(GoToNextItem);
                DiceGameManager.Instance.StartGame(enemyCharacter);

                _dialogItemsWithTriggeredDiceGame.Add(currentItem);
                return;
            }
        }

        _currentItemIndex++;

        if (_currentItemIndex >= _currentDialog.Items.Count)
        {
            Exit();
            return;
        }

        _viewer.ShowItem(_currentDialog.Items[_currentItemIndex]);
    }

    private void Exit()
    {
        PlayerMovement.Instance.EnableMovement();
        PlayerInteraction.Instance.EnableInteraction();

        _viewer.Close();

        _currentDialog = null;
        _dialogItemsWithTriggeredDiceGame.Clear();
    }
}
