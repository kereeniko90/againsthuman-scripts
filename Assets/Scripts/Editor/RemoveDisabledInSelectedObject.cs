using UnityEngine;
using UnityEditor;

public class RemoveDisabledInSelectedObject : EditorWindow
{
    [MenuItem("Tools/Remove Disabled in Selected")]
    private static void RemoveDisabledUnderSelected()
    {
        if (!EditorUtility.DisplayDialog(
                "Confirm Deletion",
                "This will delete all disabled GameObjects under the selected objects in the hierarchy. This action cannot be undone. Are you sure?",
                "Yes",
                "No"))
        {
            return;
        }

        var selectedObjects = Selection.gameObjects;
        int removedCount = 0;

        foreach (var parent in selectedObjects)
        {
            var childTransforms = parent.GetComponentsInChildren<Transform>(true);

            foreach (var child in childTransforms)
            {
                if (!child.gameObject.activeSelf)
                {
                    removedCount++;
                    Undo.DestroyObjectImmediate(child.gameObject);
                }
            }
        }

        Debug.Log($"Removed {removedCount} disabled GameObjects under the selected objects.");
    }
}
