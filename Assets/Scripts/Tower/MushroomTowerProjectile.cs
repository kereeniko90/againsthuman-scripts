
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class MushroomTowerProjectile : MonoBehaviour
{   
    [SerializeField] private MushroomTowerHitEffect hitEffectPrefab;
    
    public EnemyStatus targetEnemy;

    private Vector3 moveDirection;
    private float timeToDestroy = 3f;
    private ObjectPool<MushroomTowerProjectile> objectPool;

    public ObjectPool<MushroomTowerHitEffect> hitEffectPool;

    private MushroomTower mushroomTower;
    private int currentDamage;
    

    private bool collectionCheck = true;
    private int defaultSize = 20;
    private int maxSize = 30;
    private bool isReleased = false;
    private Vector3 hitEffectPosition;

    private void Start()
    {
        

        if (targetEnemy != null)
        {
            UpdateMoveDirection();
        }

        hitEffectPool = new ObjectPool<MushroomTowerHitEffect>(CreateHitEffect, OnGetFromPool, OnReleaseToPool, OnDestroyprojectile,collectionCheck, defaultSize, maxSize);
        

    }


    private void OnEnable()
    {   
        isReleased = false;
        StartCoroutine(DeactivateProjectileAfterTime());

    }



    private void Update()
    {
        if (targetEnemy != null)
        {
            if (targetEnemy.IsDead())
            {
                targetEnemy = null;
            }
            else
            {
                UpdateMoveDirection();
            }
        }



        float moveSpeed = 30f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        
    }

    private void UpdateMoveDirection()
    {
        if (targetEnemy != null)
        {
            float yOffset = 5.0f;
            Vector3 targetPosition = targetEnemy.transform.position + new Vector3(0, yOffset, 0);
            moveDirection = (targetPosition - transform.position).normalized;
        }
    }



    public void SetPool(ObjectPool<MushroomTowerProjectile> pool)
    {
        objectPool = pool;
    }


    private IEnumerator DeactivateProjectileAfterTime()
    {
        yield return new WaitForSeconds(timeToDestroy);
        if (!isReleased) {
            objectPool.Release(this);
        }
        


    }



    private void OnTriggerEnter(Collider collision)
    {
        EnemyStatus enemyStatus = collision.GetComponent<EnemyStatus>();
        
        if (enemyStatus != null && targetEnemy != null && enemyStatus == targetEnemy)
        {
            if (!targetEnemy.IsDead())
            {   
                
                targetEnemy.TakeDamage(currentDamage);
                SoundManager.Instance.PlaySound(SoundManager.Sound.MushroomHit);
                hitEffectPosition = collision.ClosestPoint(transform.position);
                hitEffectPool.Get();
            }

        }

        if (!isReleased) {
            objectPool.Release(this);
            isReleased = true;
        }
        
    }

    public void SetDamage(int damage)
    {
        currentDamage = damage;
    }

    #region Hit effect object pool
    private MushroomTowerHitEffect CreateHitEffect()
    {   
        Vector3 offset = new Vector3(0, 5f, 0);
        Vector3 pos = targetEnemy.transform.position + offset;
        MushroomTowerHitEffect hitEffect = Instantiate(hitEffectPrefab, hitEffectPosition, Quaternion.identity);
        hitEffect.enemyStatus = targetEnemy;
        // mushroomTowerProjectile.transform.position = attackSpawnPosition.position;
        hitEffect.SetPool(hitEffectPool);
        // mushroomTowerProjectile.SetDamage(attackDamage);


        return hitEffect;
    }

    private void OnGetFromPool(MushroomTowerHitEffect hitEffect)
    {
        Vector3 offset = new Vector3(0, 5f, 0);
        Vector3 pos = targetEnemy.transform.position + offset;
        hitEffect.enemyStatus = targetEnemy;
        hitEffect.transform.position = pos;
        hitEffect.transform.rotation = Quaternion.identity;



        hitEffect.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(MushroomTowerHitEffect hitEffect)
    {
        hitEffect.gameObject.SetActive(false);
        
        

    }

    private void OnDestroyprojectile(MushroomTowerHitEffect hitEffect)
    {
        Destroy(hitEffect.gameObject);
    }
        
    #endregion
    
    
}
