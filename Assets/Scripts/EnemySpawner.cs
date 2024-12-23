using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnterPortalPool enterPortalPrefab;
    
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private int levelIndex;
    [SerializeField] private LevelResourcesData levelResourcesData;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject waveStartObject;
    [SerializeField] private GameObject bossStartObject;

    private Animator waveStartAnimator;
    private Animator bossStartAnimator;


    //public ObjectPool<EnemyStatus> objectPool;

    private Dictionary<GameObject, ObjectPool<EnemyStatus>> objectPoolsDictionary;
    
    public ObjectPool<EnterPortalPool> enterPool;

    private bool collectionCheck = true;
    private int defaultSize = 30;
    private int maxSize = 40;
    private int maxWave;
    private int currentWave = 0;
    private bool waveStarted = false;
    private int activeEnemyCount = 0;
    private bool startWaveSound = false;
    private bool bossWaveStart = false;

    private void Awake()
    {
        maxWave = levelResourcesData.waveNumber.Count;

    }

    private void Start()
    {
        enterPool = new ObjectPool<EnterPortalPool>(CreateEnterPortalEffect, OnGetFromPortalEnterPool, OnReleaseToEnterPool, OnDestroyEffect, collectionCheck, defaultSize, maxSize);
        objectPoolsDictionary = new Dictionary<GameObject, ObjectPool<EnemyStatus>>();

        waveStartAnimator = waveStartObject.GetComponent<Animator>();
        bossStartAnimator = bossStartObject.GetComponent<Animator>();

        var levelData = levelResourcesData;

        foreach (var wave in levelData.waveNumber)
        {
            foreach (var spawnType in wave.spawnType)
            {
                if (!objectPoolsDictionary.ContainsKey(spawnType.enemyPrefabs))
                {
                    objectPoolsDictionary[spawnType.enemyPrefabs] = new ObjectPool<EnemyStatus>(
                        () => CreateEnemy(spawnType.enemyPrefabs),
                        OnGetFromPool,
                        OnReleaseToPool,
                        OnDestroyEnemy,
                        collectionCheck,
                        defaultSize,
                        maxSize
                    );
                }

            }
        }

    }

    private void Update()
    {
        if (currentWave == maxWave)
        {


            gameManager.HideStartButton();
            if (!waveStarted && activeEnemyCount == 0)
            {
                gameManager.LevelCompleted();
            }
        }
    }

    private EnemyStatus CreateEnemy(GameObject enemyPrefabs)
    {
        GameObject enemyObject = Instantiate(enemyPrefabs, spawnPosition.position, Quaternion.identity * Quaternion.Euler(0f, 90f, 0f));
        EnemyStatus enemyStatus = enemyObject.GetComponent<EnemyStatus>();
        EnterPortalPool effect = enterPool.Get();
        effect.transform.position = enemyObject.transform.position;
        enemyStatus.transform.SetParent(transform, true);

        if (objectPoolsDictionary.ContainsKey(enemyPrefabs))
        {
            enemyStatus.SetPool(objectPoolsDictionary[enemyPrefabs]);

        }


        return enemyStatus;
    }

    private void OnGetFromPool(EnemyStatus enemy)
    {
        enemy.transform.position = spawnPosition.position;
        enemy.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 90f, 0f);

        enemy.SetInitialHealthAndStatus();

        activeEnemyCount++;

        enemy.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(EnemyStatus enemy)
    {

        enemy.gameObject.SetActive(false);
        Animator animator = enemy.GetComponent<Animator>();
        //activeEnemyCount--;
        animator.SetBool("isDead", false);
    }

    private void OnDestroyEnemy(EnemyStatus enemy)
    {
        Destroy(enemy.gameObject);
    }

    private IEnumerator SpawnEnemies(LevelResourcesData.EnemyWave waveData)
    {

        waveStarted = true;

        foreach (var spawnType in waveData.spawnType)
        {
            for (int i = 0; i < spawnType.spawnCount; i++)
            {
                objectPoolsDictionary[spawnType.enemyPrefabs].Get();
                //SoundManager.Instance.PlaySound(SoundManager.Sound.EnterPortal);
                float randomTimer = Random.Range(0.1f, 2f);
                
                yield return new WaitForSeconds(randomTimer);
            }
            
        }

        waveStarted = false;


    }



    public void StartWave()
    {
        if (!waveStarted && currentWave < maxWave)
        {
            currentWave++;
            SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
            var levelData = levelResourcesData;
            StartCoroutine(SpawnEnemies(levelData.waveNumber[currentWave - 1]));
        }

        if (currentWave == 1) {
            waveStartAnimator.SetBool("show", true);
            if (!startWaveSound) {
                SoundManager.Instance.PlaySound(SoundManager.Sound.WaveStart);
                startWaveSound = true;
            }
        }

        if (currentWave == maxWave) {
            bossStartAnimator.SetBool("show", true);
            if (!bossWaveStart) {
                SoundManager.Instance.PlaySound(SoundManager.Sound.BossWave);
                bossWaveStart = true;
            }
            
        }

    }

    public bool IsWaveStarted()
    {
        return waveStarted;
    }

    public string GetCurrentWave()
    {
        return "Wave " + currentWave + "/" + maxWave;
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemyCount;
    }

    public void ReduceEnemyActiveCount()
    {
        activeEnemyCount--;
    }

    #region EnterPortalEffect

    private EnterPortalPool CreateEnterPortalEffect()
    {
        EnterPortalPool effect = Instantiate(enterPortalPrefab, spawnPosition.position, Quaternion.identity);
        effect.transform.SetParent(transform);
        effect.SetPool(enterPool);
        return effect;
    }

    private void OnGetFromPortalEnterPool(EnterPortalPool effect)
    {
        effect.transform.position = spawnPosition.position;
        effect.transform.rotation = Quaternion.identity;
        effect.PlayEffect();


        effect.gameObject.SetActive(true);
    }

    private void OnReleaseToEnterPool(EnterPortalPool effect)
    {
        effect.gameObject.SetActive(false);
        effect.transform.position = spawnPosition.position;
        effect.StopEffect();
        

    }

    private void OnDestroyEffect(EnterPortalPool effect)
    {
        Destroy(effect.gameObject);
    }

    #endregion

}
