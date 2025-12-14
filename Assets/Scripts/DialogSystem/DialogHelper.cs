using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogHelper : MonoBehaviour
{
    public static DialogHelper Instance;

    [SerializeField] private List<DialogEnemyCharacterBinding> _dialogEnemyCharacterBindings = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    public EnemyCharacter GetEnemyCharacterForDialog(string dialogName)
    {
        var matchingCollection = _dialogEnemyCharacterBindings.FirstOrDefault(binding => binding.DialogName == dialogName);
        return matchingCollection?.EnemyCharacter;
    }
}
