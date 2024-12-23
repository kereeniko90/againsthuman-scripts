using UnityEngine;


public class CactusTower : MonoBehaviour
{
    
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] public Transform attackSpawnPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private TowerData towerData;
    private float attackTimer;
    private float attackSpeed;
    private float attackRadius;
    private int attackDamage;
    private EnemyStatus targetEnemy;
    private float lookForTargetTimer;
    private float lookForTargetSpeed = 0.1f;
    private bool isAttacking;




    private void Start()
    {
        towerData = GetComponent<TowerData>();
        attackSpeed = towerData.GetAttackSpeed();
        attackRadius = towerData.GetAttackRadius();
        attackDamage = towerData.GetAttackDamage();
        animator = GetComponent<Animator>();


    }



    private void Update()
    {   
        attackSpeed = towerData.GetAttackSpeed();
        attackRadius = towerData.GetAttackRadius();
        attackDamage = towerData.GetAttackDamage();
        HandleTargetting();
        CheckTargetDistance();
        LookAtEnemy();
    }



    public void AttackEnemy()
    {

        if (targetEnemy != null && !isAttacking)
        {

            
            SpawnProjectile();

        }
        else
        {
            animator.SetBool("attack", false);
        }

    }

    private void SpawnProjectile()
    {

        int numProjectiles = 14;
        float angleStep = 360f / numProjectiles;
        float currentAngle = 0f;


        for (int i = 0; i < numProjectiles; i++)
        {

            float projectileDirX = Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float projectileDirZ = Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            Vector3 projectileDirection = new Vector3(projectileDirX, 0f, projectileDirZ);


            

            GameObject projectile = Instantiate(projectilePrefab, attackSpawnPosition.position, Quaternion.LookRotation(projectileDirection) * Quaternion.Euler(90f,0f,0f));   

            
                        
            
            CactusTowerProjectile cactusTowerProjectile = projectile.GetComponent<CactusTowerProjectile>();
            cactusTowerProjectile.SetDamage(attackDamage);
            
            
            cactusTowerProjectile.SetDirection(projectileDirection);
            currentAngle += angleStep;
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

    public EnemyStatus SetEnemyTarget()
    {
        return targetEnemy;
    }

    private void CheckTargetDistance()
    {
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distanceToEnemy > attackRadius || targetEnemy.IsDead())
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
