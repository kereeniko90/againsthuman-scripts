using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelResourcesData", menuName = "Scriptable Objects/LevelResourcesData")]
public class LevelResourcesData : ScriptableObject
{
    

    [System.Serializable]
    public class EnemyWave {
        public List<SpawnType> spawnType = new List<SpawnType>();
    }

    [System.Serializable]
    public class SpawnType {
        public GameObject enemyPrefabs;
        public int spawnCount;
    }

    
        
        public int startingGold;
        public int lifeCount;
        public int levelNumber;
        public List<EnemyWave> waveNumber = new List<EnemyWave>(); 
    
}
