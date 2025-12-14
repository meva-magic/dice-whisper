using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Character Data")]
[System.Serializable]
public class EnemyCharacterData : CharacterData
{
    [SerializeField][Range(0f, 100f)] private float _suspiciousness = 20f;
    public float Suspiciousness => _suspiciousness;

    [SerializeField][Range(0f, 1f)] private float _courage = 0.5f;
    public float Courage => _courage;
}
