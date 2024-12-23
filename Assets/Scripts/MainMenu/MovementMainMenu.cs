
using UnityEngine;

public class MovementMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject point;

    private float moveSpeed = 10f;
    private Vector3 startPosition;


    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {




        transform.position = Vector3.MoveTowards(transform.position, point.transform.position, moveSpeed * Time.deltaTime);
        transform.LookAt(point.transform.position);

        if (Vector3.Distance(transform.position, point.transform.position) < 0.01f)
        {
            transform.position = startPosition;
        }



    }
}


