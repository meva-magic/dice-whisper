using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CharacterData : ScriptableObject
{
    [SerializeField] private Sprite _uiSprite;
    public Sprite UISprite => _uiSprite;

    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField][Min(0.166f)] private float _luck = 1f;
    public float Luck => _luck;
}
