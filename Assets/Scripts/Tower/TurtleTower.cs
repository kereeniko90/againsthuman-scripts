using UnityEngine;
using UnityEngine.Pool;

public class TurtleTower : MonoBehaviour
{
    [SerializeField] private TurtleTowerProjectile projectilePrefab;
    [SerializeField] public Transform attackSpawnPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private TowerData towerData;
    
    private float attackRadius;
    private int attackDamage;
    private EnemyStatus targetEnemy;
    private float lookForTargetTimer;
    private float lookForTargetSpeed = 0.1f;
    private bool isAttacking;
    private bool collectionCheck = true;
    private int defaultSize = 20;
    private int maxSize = 30;


    public ObjectPool<TurtleTowerProjectile> objectPool;
    
    private void Start()
    {
        towerData = GetComponent<TowerData>();
        attackRadius = towerData.GetAttackRadius();
        attackDamage = towerData.GetAttackDamage();
        animator = GetComponent<Animator>();
        objectPool = new ObjectPool<TurtleTowerProjectile>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyprojectile,collectionCheck, defaultSize, maxSize);

    }


    private void Update()
    {
        
        attackRadius = towerData.GetAttackRadius();
        attackDamage = towerData.GetAttackDamage();

        HandleTargetting();
        CheckTargetDistance();
        LookAtEnemy();
    }

    private TurtleTowerProjectile CreateProjectile()
    {
        TurtleTowerProjectile cactusTowerProjectile = Instantiate(projectilePrefab, attackSpawnPosition.position, Quaternion.identity);
        cactusTowerProjectile.targetEnemy = targetEnemy;
        cactusTowerProjectile.transform.position = attackSpawnPosition.position;
        cactusTowerProjectile.SetPool(objectPool);
        cactusTowerProjectile.SetDamage(attackDamage);


        return cactusTowerProjectile;
    }

    private void OnGetFromPool(TurtleTowerProjectile projectile)
    {
        projectile.targetEnemy = targetEnemy;
        projectile.SetDamage(attackDamage);
        projectile.transform.position = attackSpawnPosition.position;
        projectile.transform.rotation = Quaternion.identity;
        projectile.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(TurtleTowerProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
        projectile.transform.position = attackSpawnPosition.position;
        projectile.targetEnemy = null;

    }

    private void OnDestroyprojectile(TurtleTowerProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    public void AttackEnemy()
    {
        if (targetEnemy != null && !isAttacking)
        {
            isAttacking = true;
            objectPool.Get();

        }
        else
        {
            animator.SetBool("attack", false);
        }
    }

    private void LookAtEnemy()
    {
        if (targetEnemy == null) return;

        transform.LookAt(targetEnemy.transform);

        animator.SetBool("attack", true);
    }

    private void HandleTargetting()
    {
        lookForTargetTimer -= Time.deltaTime;

        if (lookForTargetTimer < 0f)
        {

            SetTarget();
            lookForTargetTimer += lookForTargetSpeed;
        }
    }

    private void SetTarget()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, attackRadius);

        targetEnemy = null;

        foreach (Collider collider in colliderArray)
        {
            EnemyStatus enemy = collider.GetComponent<EnemyStatus>();
            if (enemy != null && !enemy.IsDead())
            {

                if (targetEnemy == null)
                {
                    targetEnemy = enemy;
                }
                else
                {
                    if (Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, targetEnemy.transform.position))
                    {
                        targetEnemy = enemy;
                    }
                }



            }

        }
    }



    public EnemyStatus GetEnemyTarget()
    {
        return targetEnemy;
    }

    private void CheckTargetDistance()
    {
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distanceToEnemy > attackRadius)
            {

                targetEnemy = null;
                isAttacking = false;
                animator.SetBool("attack", false);
            }
        }
    }

    public void ResetAttacking()
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public int GetDamage()
    {
        return attackDamage;
    }
}
