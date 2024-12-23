using UnityEngine;
using UnityEditor;

public class RandomYRotationTool : EditorWindow
{
    private float minRotation = 0f;
    private float maxRotation = 360f;

    [MenuItem("Tools/Random Y Rotation Tool")]
    public static void ShowWindow()
    {
        GetWindow<RandomYRotationTool>("Random Y Rotation Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Random Y Rotation Tool", EditorStyles.boldLabel);
        
        // Fields for specifying the range of random rotation
        minRotation = EditorGUILayout.FloatField("Min Rotation (Y)", minRotation);
        maxRotation = EditorGUILayout.FloatField("Max Rotation (Y)", maxRotation);

        // Button to apply random rotation
        if (GUILayout.Button("Apply Random Y Rotation"))
        {
            AssignRandomYRotationToSelectedObjects();
        }
    }

    private void AssignRandomYRotationToSelectedObjects()
    {
        if (Selection.transforms.Length == 0)
        {
            Debug.LogWarning("No objects selected! Please select objects to apply random Y rotation.");
            return;
        }

        foreach (Transform transform in Selection.transforms)
        {
            Undo.RecordObject(transform, "Randomize Y Rotation");
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y = Random.Range(minRotation, maxRotation);
            transform.eulerAngles = currentRotation;

            EditorUtility.SetDirty(transform);
        }

        Debug.Log($"Applied random Y rotation to {Selection.transforms.Length} object(s).");
    }
}

