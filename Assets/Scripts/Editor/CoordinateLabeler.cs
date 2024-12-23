using UnityEngine;
using TMPro;

[ExecuteAlways]
public class CoordinateLabeler : MonoBehaviour {

    private TextMeshPro label;
    Vector2Int coordinates = new Vector2Int();

    private void Awake() {
        label = GetComponent<TextMeshPro>();//test
        DisplayCoordinates();
    }
    
    void Update() {
        if (!Application.isPlaying) {
            DisplayCoordinates();
            UpdateObjectName();
        }
    }

    private void DisplayCoordinates() {

        coordinates.x = Mathf.RoundToInt(transform.parent.position.x / UnityEditor.EditorSnapSettings.move.x);
        coordinates.y = Mathf.RoundToInt(transform.parent.position.z / UnityEditor.EditorSnapSettings.move.z);

        label.text = coordinates.x + "," + coordinates.y;
    }

    private void UpdateObjectName() {
        transform.parent.name = coordinates.ToString();
    }
}
