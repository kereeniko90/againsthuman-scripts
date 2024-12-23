using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class EnterPortalPool : MonoBehaviour
{
    private ObjectPool<EnterPortalPool> enterPool;
    private ParticleSystem particleEffect;
    private Coroutine deactivateCoroutine;

    private void Start() {
        
            
        
        
    }

    private void OnEnable() {
        particleEffect = GetComponent<ParticleSystem>();
        if (particleEffect != null) {
            particleEffect.Play();
        }

        // Stop any existing coroutine and start a new one
        if (deactivateCoroutine != null) {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateEffectAfterTime());
    }
    
    private void OnDisable() {
        if (deactivateCoroutine != null) {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }
    }

    public void SetPool(ObjectPool<EnterPortalPool> pool) {
        enterPool = pool;
    }

    private IEnumerator DeactivateEffectAfterTime()
    {
        yield return new WaitForSeconds(1f);

        
        if (particleEffect != null) {
            particleEffect.Stop();
        }

        
        if (enterPool != null) {
            enterPool.Release(this);
        }        
    }

    public void PlayEffect() {
        particleEffect.Play();
    }

    public void StopEffect() {
        particleEffect.Stop();
    }
}
