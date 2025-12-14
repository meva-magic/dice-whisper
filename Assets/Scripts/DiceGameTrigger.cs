using UnityEngine;

public class DiceGameTrigger : MonoBehaviour
{
    [SerializeField] private EnemyCharacter _enemyCharacter;

    public void StartDiceGame()
    {
        DiceGameManager.Instance.StartGame(_enemyCharacter);
    }
}
