using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class MushroomTowerHitEffect : MonoBehaviour
{
    private ObjectPool<MushroomTowerHitEffect> objectPool;
    private float timeToDestroy = 0.3f;
    public EnemyStatus enemyStatus;
    private ParticleSystem particle;

    

    private void Start() {
        //StartCoroutine(DeactivateAfterTime());
        particle = GetComponent<ParticleSystem>();
    }

    private void Update() {
        
        Vector3 offset = new Vector3(0, 5f, 0);
        Vector3 pos = enemyStatus.transform.position + offset;
        transform.position = pos;

        if (particle.isStopped) {
            objectPool.Release(this);
        }
    }

    public void SetPool(ObjectPool<MushroomTowerHitEffect> pool) {
        objectPool = pool;
    }

    public void ReleaseFromPool() {
        objectPool.Release(this);
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(timeToDestroy);
        objectPool.Release(this);


    }
}
