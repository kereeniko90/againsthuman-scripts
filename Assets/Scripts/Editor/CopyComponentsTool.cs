using UnityEngine;
using UnityEditor;

public class CopyComponentsTool : EditorWindow
{
    private GameObject sourceObject;
    private GameObject targetObject;

    [MenuItem("Tools/Copy Components Tool")]
    public static void ShowWindow()
    {
        GetWindow<CopyComponentsTool>("Copy Components Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Components from One GameObject to Another", EditorStyles.boldLabel);

        sourceObject = (GameObject)EditorGUILayout.ObjectField("Source Object", sourceObject, typeof(GameObject), true);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Copy Components"))
        {
            if (sourceObject == null || targetObject == null)
            {
                Debug.LogError("Please assign both a Source Object and a Target Object.");
            }
            else
            {
                CopyComponents(sourceObject, targetObject);
            }
        }
    }

    private void CopyComponents(GameObject source, GameObject target)
    {
        var components = source.GetComponents<Component>();

        foreach (var component in components)
        {
            if (component is Transform) continue; // Skip the Transform component

            System.Type type = component.GetType();
            var existingComponent = target.GetComponent(type);

            if (existingComponent == null)
            {
                var newComponent = target.AddComponent(type);
                EditorUtility.CopySerialized(component, newComponent);
            }
            else
            {
                EditorUtility.CopySerialized(component, existingComponent);
            }
        }

        Debug.Log($"Copied components from {source.name} to {target.name}.");
    }
}
