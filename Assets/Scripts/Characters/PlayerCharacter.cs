using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private PlayerCharacterData _playerCharacterData;
    public PlayerCharacterData CharacterData => _playerCharacterData;

    protected override void OnInit()
    {
        SetCharacterData(_playerCharacterData);
    }

    public void SaveScoreToCharacterData()
    {
        _playerCharacterData.Score += _score;
    }
}
