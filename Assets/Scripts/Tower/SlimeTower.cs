using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class SlimeTower : MonoBehaviour
{
    [SerializeField] private SlimeTowerProjectile projectilePrefab;
    [SerializeField] public Transform attackSpawnPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private TowerData towerData;

    private List<EnemyStatus> targetEnemies = new List<EnemyStatus>();
    private List<EnemyStatus> chosenEnemies = new List<EnemyStatus>();

    private float attackRadius;
    private int attackDamage;
    private EnemyStatus targetEnemy;
    private float lookForTargetTimer;
    private float lookForTargetSpeed = 0.1f;
    private bool isAttacking;

    private bool collectionCheck = true;
    private int defaultSize = 20;
    private int maxSize = 30;


    public ObjectPool<SlimeTowerProjectile> objectPool;




    private void Start()
    {
        towerData = GetComponent<TowerData>();

        attackRadius = towerData.GetAttackRadius();
        attackDamage = towerData.GetAttackDamage();
        animator = GetComponent<Animator>();
        objectPool = new ObjectPool<SlimeTowerProjectile>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyprojectile, collectionCheck, defaultSize, maxSize);

    }


    private void Update()
    {

        attackRadius = towerData.GetAttackRadius();
        attackDamage = towerData.GetAttackDamage();



        HandleTargetting();
        CheckTargetDistance();
        LookAtEnemy();
    }

    private SlimeTowerProjectile CreateProjectile()
    {
        SlimeTowerProjectile slimeTowerProjectile = Instantiate(projectilePrefab, attackSpawnPosition.position, Quaternion.identity);
        slimeTowerProjectile.targetEnemy = targetEnemy;
        slimeTowerProjectile.transform.position = attackSpawnPosition.position;
        slimeTowerProjectile.SetPool(objectPool);
        slimeTowerProjectile.SetDamage(attackDamage);


        return slimeTowerProjectile;
    }

    private void OnGetFromPool(SlimeTowerProjectile projectile)
    {
        
        projectile.SetDamage(attackDamage);
        projectile.transform.position = attackSpawnPosition.position;
        projectile.transform.rotation = Quaternion.identity;



        projectile.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(SlimeTowerProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
        projectile.transform.position = attackSpawnPosition.position;
        projectile.targetEnemy = null;

    }

    private void OnDestroyprojectile(SlimeTowerProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    public void AttackEnemy()
    {

        

        if (targetEnemies.Count > 0 && !isAttacking)
        {

            isAttacking = true;
            foreach (EnemyStatus enemy in targetEnemies)
            {
                var projectile = objectPool.Get();
                projectile.SetTarget(enemy);
            }



        }
        else
        {
            animator.SetBool("attack", false);

        }

    }

    private void LookAtEnemy()
    {
        

        if (targetEnemies == null || targetEnemies.Count == 0)
        {
            animator.SetBool("attack", false);
            return;
        }

        EnemyStatus closestEnemy = targetEnemies[0];
        float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

        foreach (var enemy in targetEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distanceToEnemy;
            }
        }

        Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

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
        targetEnemies.Clear();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, attackRadius);

        
        chosenEnemies.Clear();

        
        foreach (Collider collider in colliderArray)
        {
            EnemyStatus enemy = collider.GetComponent<EnemyStatus>();
            if (enemy != null && !enemy.IsDead())
            {

                chosenEnemies.Add(enemy);



            }

        }

        chosenEnemies.Sort((enemyA, enemyB) =>

            Vector3.Distance(transform.position, enemyA.transform.position)
            .CompareTo(Vector3.Distance(transform.position, enemyB.transform.position))

        );

        for (int i = 0; i < Mathf.Min(3, chosenEnemies.Count); i++)
        {
            targetEnemies.Add(chosenEnemies[i]);
        }
    }



    public EnemyStatus GetEnemyTarget()
    {
        return targetEnemy;
    }

    private void CheckTargetDistance()
    {
        

        for (int i = targetEnemies.Count - 1; i >= 0; i--)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemies[i].transform.position);

            if (distanceToEnemy > attackRadius)
            {
                targetEnemies.RemoveAt(i);
            }
        }

        if (targetEnemies.Count == 0)
        {
            isAttacking = false;
            animator.SetBool("attack", false);
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
