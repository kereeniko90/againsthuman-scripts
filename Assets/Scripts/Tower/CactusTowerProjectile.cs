using System.Collections;

using UnityEngine;


public class CactusTowerProjectile : MonoBehaviour
{

    private Vector3 moveDir;
    private Vector3 initialPosition;

    [SerializeField] private GameObject cactusTowerHitEffect;

    private float moveSpeed = 30f;
    
    private float maxDistance = 25f;
    private int currentDamage;

    //private Coroutine deactivateProjectileAfterTimeCoroutine;
    private CactusTower cactusTower;

    private void Start()
    {
        
        initialPosition = transform.position;
    }



    private void Update()
    {
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceTraveled = Vector3.Distance(initialPosition, transform.position);
        if (distanceTraveled > maxDistance) {
            Destroy(gameObject);
            
        }

        
    }



    public void SetDirection(Vector3 direction)
    {
        moveDir = direction.normalized;
        

        
    }


    private void OnTriggerEnter(Collider collision)
    {
        EnemyStatus enemyStatus = collision.GetComponent<EnemyStatus>();
        

        if (enemyStatus != null)
        {   
            Vector3 point = collision.ClosestPoint(enemyStatus.transform.position);
            
            enemyStatus.TakeDamage(currentDamage);
            SoundManager.Instance.PlaySound(SoundManager.Sound.GlassHit);
            Destroy(gameObject);

            if (!enemyStatus.IsDead()) {
                Instantiate(cactusTowerHitEffect, point, Quaternion.identity);
            }
            
        }
    }

    public void SetDamage(int damage) {
        currentDamage = damage;
    }


}
