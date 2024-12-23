using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;


public class EnemyStatus : MonoBehaviour
{
    [SerializeField] public Enemy enemy;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Transform damageTextPosition;
    [SerializeField] private GameObject blockEffectPrefab;
    
    private GameObject blockEffect;
    private ParticleSystem blockEffectParticles;
    
    private BoxCollider enemyCollider;
    private EnemyMover enemyMover;
    private DamageText damageText;
    private TextMeshPro damagedTextFont;
    private float missChance;
    private float blockChance;
    [SerializeField]private Color hitColor;
    [SerializeField]private Color blockedColor;
    [SerializeField]private Color missedColor;

    private float health;
    private Animator animator;
    private bool isDead = false;
    
    
    

    private ObjectPool<EnemyStatus> objectPool;
    private ObjectPool<TextMeshPro> textMeshProPool;


    private void Start()
    {   
        
        
        
        blockEffectParticles.Stop();
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        enemyMover = GetComponent<EnemyMover>();
        damageText = GetComponent<DamageText>();
        damagedTextFont = GetComponent<TextMeshPro>();
        blockChance = enemy.blockChance;
        missChance = enemy.missChance;

        health = enemy.enemyHealth;
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        if (blockEffect == null) {
            blockEffect = Instantiate(blockEffectPrefab, transform.position, Quaternion.identity);
            blockEffect.transform.SetParent(transform, true);
        }

        
        
        blockEffectParticles = blockEffect.GetComponent<ParticleSystem>();
        blockEffectParticles.Stop();
        
        enemyCollider = GetComponent<BoxCollider>();
        enemyCollider.enabled = true;
    }



    private void Update()
    {

        if (health <= 0 && !isDead)
        {

            animator.SetBool("isDead", true);
            gameManager.AddCoin(enemy.enemyGoldDrop);
            enemyCollider.enabled = false;
            isDead = true;
            if (enemy.voiceType == Enemy.VoiceType.Male) {
                SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyMaleDie);
            } else {
                SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyFemaleDie);
            }
            enemySpawner.ReduceEnemyActiveCount();
            StartCoroutine(ReleaseAfterDelay());
        }
    }

    public void TakeDamage(int damage)
    {

        if (health > 0)
        {   
            if (ShouldHit()) {

                if (ShouldReduceDamage()) {
                    
                    damageText.CreateTextDamage(damageTextPosition.position, "Block", blockedColor);
                    SoundManager.Instance.PlaySound(SoundManager.Sound.Block);
                    blockEffectParticles.Play();
                    
                } else {
                    health -= damage;
                    SoundManager.Instance.PlaySound(SoundManager.Sound.Evade);
                    damageText.CreateTextDamage(damageTextPosition.position, damage.ToString(), hitColor);
                    
                }
                
            } else {
                damageText.CreateTextDamage(damageTextPosition.position, "Miss", missedColor);
                
            }

            // health -= damage;
            // damageText.CreateTextDamage(damageTextPosition.position, damage.ToString());
        }

    }

    private bool ShouldReduceDamage() {
        float randomValue = Random.Range(0,100);
        return randomValue < blockChance;
    }

    private bool ShouldHit () {
        float randomValue = Random.Range(0,100);
        return randomValue >= missChance;
    }

    public bool IsDead()
    {
        return isDead;
    }
    //
    public void SetPool(ObjectPool<EnemyStatus> pool)
    {
        objectPool = pool;
        
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Exit"))
        {
            Debug.Log("hit");
            objectPool.Release(this);
        }
    }

    public void SetInitialHealthAndStatus()
    {
        health = enemy.enemyHealth;
        isDead = false;
    }



    private IEnumerator ReleaseAfterDelay()
    {

        if (objectPool == null)
        {
            
            yield break;
        }

        yield return new WaitForSeconds(3f);

        Vector3 pos = transform.position;

        float descentDuration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < descentDuration)
        {
            pos.y -= 3f * Time.deltaTime;

            transform.position = pos;

            elapsedTime += Time.deltaTime;

            yield return null;
        }


        yield return new WaitForSeconds(5f);



        objectPool.Release(this);
    }

    public void ReleaseThisObject()
    {
        objectPool.Release(this);
    }

    public void SlowedDown() {
        enemyMover.HalfMoveSpeed();
    }

    public void PlayDeathSound() {
        if (enemy.voiceType == Enemy.VoiceType.Male) {
                SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyMaleDie);
            } else {
                SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyFemaleDie);
            }
    }

    
}
