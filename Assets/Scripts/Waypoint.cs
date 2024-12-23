using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private bool isPlaceable;
    [SerializeField] private LayerMask environmentLayer;

    bool m_started = false;
    private void Start() {
        CheckSurrounding();
        m_started = true;
    }
    private void OnMouseDown() {
        if (isPlaceable) {
            
        }
    }

    public bool IsPlaceable() {
        return isPlaceable;
    }

    public void DisablePlaceable () {
        isPlaceable = false;
        
    }

    

    private void CheckSurrounding()
    {   
        Vector3 offset = new Vector3 (0,1f,0);
        
        Collider[] colliderArray = Physics.OverlapBox(transform.position, new Vector3(10,1.5f,10) / 2, Quaternion.identity, environmentLayer);
        
        
        if (colliderArray.Length > 0) {
            DisablePlaceable();
        }

       
    }

    private void OnDrawGizmosSelected() {
        // Debugging: Draw the OverlapSphere in the editor
        Vector3 offset = new Vector3(0, 0.1f, 0);
        
        Gizmos.color = Color.red;

        if (m_started) {
            Gizmos.DrawWireCube(transform.position, new Vector3(10,1.5f,10));
        }
        
    }
}
