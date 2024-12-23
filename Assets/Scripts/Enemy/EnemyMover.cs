using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    
    [SerializeField][Range(0f, 5f)] private float speed = 1f;
    [SerializeField][Range(0f, 500f)] private float rotationSpeed = 90f;

    [SerializeField] private GameObject slowDebuffPrefab;

    private Level_01 levelPath;
    private List<Waypoint> path;
    private EnemyStatus enemyStatus;

    private float slowTimer = 0;
    private float slowTimerDuration = 4f;
    private bool isSlowed = false;
    private float currentSpeed;
    public GameObject slowDebuff;

    private void Awake() {
        
    }

    private void Start()
    {   
        //slowDebuff = Instantiate(slowDebuffPrefab, transform.position, Quaternion.identity);
        //slowDebuff.transform.SetParent(transform, true);
        slowDebuff.SetActive(false);
        levelPath = FindAnyObjectByType<Level_01>();
        enemyStatus = GetComponent<EnemyStatus>();
        speed = enemyStatus.enemy.enemyMoveSpeed;
        path = levelPath.WaypointList();
        StartCoroutine(FollowPath());
    }

    private void OnEnable() {
        slowDebuff = Instantiate(slowDebuffPrefab, transform.position, Quaternion.identity);
        slowDebuff.transform.SetParent(transform, true);
        //slowDebuff.transform.SetParent(transform, false);
        slowDebuff.SetActive(false);
        levelPath = FindAnyObjectByType<Level_01>();
        enemyStatus = GetComponent<EnemyStatus>();
        speed = enemyStatus.enemy.enemyMoveSpeed;
        path = levelPath.WaypointList();
        StartCoroutine(FollowPath());
    }

    private void Update() {
        if (isSlowed) {
            slowTimer += Time.deltaTime;
            currentSpeed = speed / 2;
            slowDebuff.SetActive(true);

            if (slowTimer >= slowTimerDuration) {
                isSlowed = false;
            }
        } else {
            currentSpeed = speed;
            slowTimer = 0;
            slowDebuff.SetActive(false);
        }
    }



    private IEnumerator FollowPath()
    {


        foreach (Waypoint waypoint in path)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = waypoint.transform.position;
            float travelPercent = 0f;



            while (travelPercent < 1f)
            {
                if (enemyStatus.IsDead())
                {
                    yield break;
                }
                travelPercent += Time.deltaTime * currentSpeed;
                transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);

                Vector3 direction = endPosition - transform.position;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                
                yield return null;
            }

        }
    }

    public void HalfMoveSpeed() {
        isSlowed = true;
    }


}
