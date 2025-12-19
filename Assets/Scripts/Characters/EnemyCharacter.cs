using UnityEngine;

public class EnemyCharacter : Character
{
    public const float MaxSuspicion = 100f;

    [SerializeField] private EnemyCharacterData _enemyCharacterData;
    public EnemyCharacterData CharacterData => _enemyCharacterData;
    public float Suspiciousness => _enemyCharacterData.Suspiciousness;
    public float Courage => _enemyCharacterData.Courage;

    protected float _suspicion;
    public float Suspicion => _suspicion;

    protected override void OnInit()
    {
        SetCharacterData(_enemyCharacterData);

        _suspicion = 0;
    }

    public void AddSuspicion()
    {
        _suspicion = Mathf.Clamp
        (
            value: _suspicion + Suspiciousness,
            min: _suspicion,
            max: MaxSuspicion
        );

        DiceGameManager.Instance.UpdateEnemySuspicionUI();

        if (_suspicion == MaxSuspicion)
            DiceGameManager.Instance.CheatingExposed();
    }
}
