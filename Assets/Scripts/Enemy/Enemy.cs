using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    public string enemyName;
    public int enemyHealth;
    public int enemyGoldDrop;
    public float blockChance;
    public float missChance;
    public EnemyType enemyType;
    public VoiceType voiceType;
    [Range(0f, 5f)] public float enemyMoveSpeed;

    [System.Serializable]
    public enum EnemyType {
        Normal,
        Boss,
    }

    [System.Serializable]
    public enum VoiceType {
        Male,
        Female
    }
}
