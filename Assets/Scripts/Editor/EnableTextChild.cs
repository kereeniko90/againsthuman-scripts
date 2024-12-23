using UnityEngine;
using UnityEditor;

public class EnableTextChild : MonoBehaviour
{
    [MenuItem("Tools/Enable Text in Selected")]
    private static void DisableTextInSelected()
    {
        
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected. Please select objects in the hierarchy.");
            return;
        }

        foreach (GameObject obj in selectedObjects)
        {
            
            Transform textChild = obj.transform.Find("Text");

            if (textChild != null)
            {
                
                textChild.gameObject.SetActive(true);
                Debug.Log($"Enabled 'Text' GameObject in {obj.name}");
            }
            else
            {
                Debug.LogWarning($"No child named 'Text' found in {obj.name}");
            }
        }
    }
}
