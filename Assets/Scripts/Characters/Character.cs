using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected const float MaxScore = 100f;

    private CharacterData _characterData;
    public Sprite UISprite => _characterData.UISprite;
    public string Name => _characterData.Name;
    public float Luck => _characterData.Luck;

    protected int _score;
    public int Score => _score;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _score = 0;
        OnInit();
    }

    protected abstract void OnInit();

    public void SetCharacterData(CharacterData characterData)
    {
        if (characterData == null)
            return;

        _characterData = characterData;
    }

    public int RollDice(bool guaranteeSuccess = false)
    {
        if (!guaranteeSuccess)
        {
            float rollOneChance = (1f / 6f) / Luck;

            if (Random.value <= rollOneChance)
                return 1;
        }

        float roll = Random.value;

        return roll switch
        {
            >= 0f and <= 0.2f => 2,
            > 0.2f and <= 0.4f => 3,
            > 0.4f and <= 0.6f => 4,
            > 0.6f and <= 0.8f => 5,
            _ => 6,
        };
    }

    public void AddScore(int amount)
    {
        _score += amount;

        DiceGameManager.Instance.UpdateCharacterScoreUI(this);

        if (_score >= MaxScore)
            DiceGameManager.Instance.CharacterWon(this);
    }
}
