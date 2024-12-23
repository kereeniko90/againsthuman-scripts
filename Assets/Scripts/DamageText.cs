using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform spawnPosition;
    private ObjectPool<GameObject> objectPool;
    private TextMeshPro damageText;

    private bool collectionCheck = true;
    private int defaultSize = 10;
    private int maxSize = 10;
    private float timeToDestroy = 2f;


    private void Awake()
    {

    }


    private void OnEnable()
    {



    }

    private void Start()
    {
        if (objectPool == null)
        {
            objectPool = new ObjectPool<GameObject>(CreateText, OnGetFromPool, OnReleaseToPool, OnDestroyprojectile, collectionCheck, defaultSize, maxSize);
        }
        EnemyStatus enemy = GetComponent<EnemyStatus>();



    }

    private void Update()
    {

    }

    private GameObject CreateText()
    {
        GameObject damageText = Instantiate(damageTextPrefab);
        damageText.transform.SetParent(transform, true);



        return damageText;
    }

    private void OnGetFromPool(GameObject damageText)
    {
        // Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1f), Random.Range(-0.5f, 0.5f));
        // Vector3 randomizedPosition = spawnPosition.position + randomOffset;
        // damageText.transform.position = randomizedPosition;

        damageText.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(GameObject damageText)
    {
        damageText.gameObject.SetActive(false);
    }

    private void OnDestroyprojectile(GameObject damageText)
    {
        Destroy(damageText.gameObject);
    }

    private void SetDamageText(GameObject damageText, string damageAmount)
    {
        TextMeshPro textMesh = damageText.GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = damageAmount;
        }
    }

    public void CreateTextDamage(Vector3 position, string damageAmount, Color color)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1f), Random.Range(-0.5f, 0.5f));

        Vector3 randomizedPosition = position + randomOffset;
        
        GameObject damageText = objectPool.Get();
        damageText.transform.position = randomizedPosition;
        TextMeshPro font = damageText.GetComponent<TextMeshPro>();
        font.color = color;

        

        SetDamageText(damageText, damageAmount);
        StartCoroutine(DeactivateProjectileAfterTime(damageText));

    }

    // private IEnumerator DeactivateProjectileAfterTime(GameObject obj)
    // {   Vector3 upMovement = new Vector3(0, 0.1f, 0);
    //     float elapsedTime = 0f;
    //     while (elapsedTime < timeToDestroy)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         obj.transform.position += upMovement;
    //         yield return null;
    //     }

    //     objectPool.Release(gameObject);
    // }



    private IEnumerator DeactivateProjectileAfterTime(GameObject obj)
    {
        Vector3 startPosition = obj.transform.position;
        float elapsedTime = 0f;

        
        float horizontalSpeed = 20f; 
        float arcHeight = 1f; // 

        while (elapsedTime < timeToDestroy)
        {
            // Calculate the normalized time (0 to 1)
            float normalizedTime = elapsedTime / timeToDestroy;

            // Horizontal movement (linear)
            float xOffset = normalizedTime * horizontalSpeed;

            // Vertical arc movement (using sine function for smooth arc)
            float yOffset = 4 * Mathf.Sin(normalizedTime * Mathf.PI) * arcHeight;

            // Update the position
            obj.transform.position = startPosition + new Vector3(xOffset, yOffset, 0);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Release the object back to the pool
        objectPool.Release(obj);
    }



}
