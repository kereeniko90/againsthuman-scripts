using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class SlimeTowerProjectile : MonoBehaviour
{
    [SerializeField] private SlimeTowerHitEffect hitEffectPrefab;
    
    public EnemyStatus targetEnemy;

    private Vector3 moveDirection;
    private float timeToDestroy = 3f;
    private ObjectPool<SlimeTowerProjectile> objectPool;

    public ObjectPool<SlimeTowerHitEffect> hitEffectPool;

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

        hitEffectPool = new ObjectPool<SlimeTowerHitEffect>(CreateHitEffect, OnGetFromPool, OnReleaseToPool, OnDestroyprojectile,collectionCheck, defaultSize, maxSize);
        

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

    public void SetTarget(EnemyStatus target)
{
    targetEnemy = target;
}



    public void SetPool(ObjectPool<SlimeTowerProjectile> pool)
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
                targetEnemy.SlowedDown();
                SoundManager.Instance.PlaySound(SoundManager.Sound.IceHit);

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
    private SlimeTowerHitEffect CreateHitEffect()
    {   
        Vector3 offset = new Vector3(0, 5f, 0);
        Vector3 pos = targetEnemy.transform.position + offset;
        SlimeTowerHitEffect hitEffect = Instantiate(hitEffectPrefab, hitEffectPosition, Quaternion.identity);
        hitEffect.enemyStatus = targetEnemy;
        
        hitEffect.SetPool(hitEffectPool);
        


        return hitEffect;
    }

    private void OnGetFromPool(SlimeTowerHitEffect hitEffect)
    {
        Vector3 offset = new Vector3(0, 5f, 0);
        Vector3 pos = targetEnemy.transform.position + offset;
        hitEffect.enemyStatus = targetEnemy;
        hitEffect.transform.position = pos;
        hitEffect.transform.rotation = Quaternion.identity;



        hitEffect.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(SlimeTowerHitEffect hitEffect)
    {
        hitEffect.gameObject.SetActive(false);
        
        

    }

    private void OnDestroyprojectile(SlimeTowerHitEffect hitEffect)
    {
        Destroy(hitEffect.gameObject);
    }
        
    #endregion
}
