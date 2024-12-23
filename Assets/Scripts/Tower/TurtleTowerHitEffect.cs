using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class TurtleTowerHitEffect : MonoBehaviour
{
    private ObjectPool<TurtleTowerHitEffect> objectPool;
    private float timeToDestroy = 0.3f;
    public EnemyStatus enemyStatus;
    private ParticleSystem particle;

    

    private void Start() {
        //StartCoroutine(DeactivateAfterTime());
        particle = GetComponent<ParticleSystem>();
    }

    private void Update() {
                
        if (particle.isStopped) {
            objectPool.Release(this);
        }
    }

    public void SetPool(ObjectPool<TurtleTowerHitEffect> pool) {
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
