using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class TurtleTowerProjectile : MonoBehaviour
{
    [SerializeField] private TurtleTowerHitEffect hitEffectPrefab;
    public EnemyStatus targetEnemy;
    private Vector3 moveDirection;
    private float timeToDestroy = 1f;
    private ObjectPool<TurtleTowerProjectile> objectPool;
    public ObjectPool<TurtleTowerHitEffect> hitEffectPool;
    private int currentDamage;
    private int enemyHitCount = 0;
    private int maxHitCount = 3;
    private bool collectionCheck = true;
    private int defaultSize = 20;
    private int maxSize = 30;
    private bool isReleased = false;
    private Vector3 hitEffectPosition;

    private void Start()
    {
        if (targetEnemy != null)
        {
            SetInitialDirection();
        }
        hitEffectPool = new ObjectPool<TurtleTowerHitEffect>(CreateHitEffect, OnGetFromPool, OnReleaseToPool, OnDestroyprojectile, collectionCheck, defaultSize, maxSize);
    }


    private void OnEnable()
    {
        isReleased = false;
        enemyHitCount = 0;
        SetInitialDirection();
        StartCoroutine(DeactivateProjectileAfterTime());
    }



    private void Update()
    {

        float moveSpeed = 40f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void SetInitialDirection()
    {
        if (targetEnemy != null)
        {
            float yOffset = 5.0f;
            Vector3 targetPosition = targetEnemy.transform.position + new Vector3(0, yOffset, 0);
            moveDirection = (targetPosition - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }


    public void SetPool(ObjectPool<TurtleTowerProjectile> pool)
    {
        objectPool = pool;
    }


    private IEnumerator DeactivateProjectileAfterTime()
    {
        yield return new WaitForSeconds(timeToDestroy);
        if (!isReleased)
        {
            objectPool.Release(this);
        }

    }



    private void OnTriggerEnter(Collider collision)
    {
        EnemyStatus enemyStatus = collision.GetComponent<EnemyStatus>();

        if (enemyStatus != null)
        {
            if (!enemyStatus.IsDead())
            {

                enemyStatus.TakeDamage(currentDamage);
                SoundManager.Instance.PlaySound(SoundManager.Sound.TurtleHit);
                enemyHitCount++;
                hitEffectPosition = collision.ClosestPoint(transform.position);
                hitEffectPool.Get();
            }





        }

        if (enemyHitCount >= maxHitCount)
        {
            if (!isReleased)
            {
                objectPool.Release(this);
                isReleased = true;
            }
        }



    }

    public void SetDamage(int damage)
    {
        currentDamage = damage;
    }

    #region Hit effect object pool
    private TurtleTowerHitEffect CreateHitEffect()
    {
        
        Vector3 pos = hitEffectPosition;
        TurtleTowerHitEffect hitEffect = Instantiate(hitEffectPrefab, hitEffectPosition, Quaternion.identity);
        hitEffect.SetPool(hitEffectPool);

        return hitEffect;
    }

    private void OnGetFromPool(TurtleTowerHitEffect hitEffect)
    {
        
        Vector3 pos = hitEffectPosition;
        hitEffect.transform.position = pos;
        hitEffect.transform.rotation = Quaternion.identity;
        hitEffect.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(TurtleTowerHitEffect hitEffect)
    {
        hitEffect.gameObject.SetActive(false);

    }

    private void OnDestroyprojectile(TurtleTowerHitEffect hitEffect)
    {
        Destroy(hitEffect.gameObject);
    }

    #endregion


}
