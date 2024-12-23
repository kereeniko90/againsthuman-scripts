using UnityEngine;
using System.Collections;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ExitPortal : MonoBehaviour
{   
    [SerializeField] private ExitPortalPool exitPortalPrefab;
    private GameManager gameManager;
    private EnemySpawner enemySpawner;

    public ObjectPool<ExitPortalPool> exitPool;

    private bool collectionCheck = true;
    private int defaultSize = 30;
    private int maxSize = 40;

    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemySpawner = GameObject.Find("EnemySpawner").GetComponentInParent<EnemySpawner>();
        exitPool = new ObjectPool<ExitPortalPool>(CreateExitPortalEffect, OnGetFromPortalExitPool, OnReleaseToExitPool, OnDestroyEffect, collectionCheck, defaultSize, maxSize);
    }
    private void OnTriggerEnter(Collider collision) {
        EnemyStatus enemyStatus = collision.GetComponent<EnemyStatus>();
        
        
        if (enemyStatus != null) {
            gameManager.ReduceLife(enemyStatus);
            enemyStatus.ReleaseThisObject();
            //SoundManager.Instance.PlaySound(SoundManager.Sound.ExitPortal);
            enemySpawner.ReduceEnemyActiveCount();
            exitPool.Get();
        }
    }

    
    #region ExitPortalEffect

    private ExitPortalPool CreateExitPortalEffect()
    {
        ExitPortalPool effect = Instantiate(exitPortalPrefab, transform.position, Quaternion.identity);
        effect.transform.SetParent(transform);
        effect.SetPool(exitPool);
        return effect;
    }

    private void OnGetFromPortalExitPool(ExitPortalPool effect)
    {
        effect.transform.position = transform.position;
        effect.transform.rotation = Quaternion.identity;
        effect.PlayEffect();


        effect.gameObject.SetActive(true);
    }

    private void OnReleaseToExitPool(ExitPortalPool effect)
    {
        effect.gameObject.SetActive(false);
        effect.transform.position = transform.position;
        effect.StopEffect();

    }

    private void OnDestroyEffect(ExitPortalPool effect)
    {
        Destroy(effect.gameObject);
    }

    #endregion

}

