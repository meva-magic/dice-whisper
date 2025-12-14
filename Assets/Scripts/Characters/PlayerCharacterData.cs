using UnityEngine;

[CreateAssetMenu(menuName = "Player Character Data")]
[System.Serializable]
public class PlayerCharacterData : CharacterData
{
    [SerializeField][Min(0f)] private float _score = 0f;
    public float Score
    {
        get => _score;
        set
        {
            if (value >= 0f)
                _score = value;
        }
    }
}
