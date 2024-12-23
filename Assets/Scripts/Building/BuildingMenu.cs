using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingMenu : MonoBehaviour, IPointerEnterHandler
{   
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private CustomCursor customCursor;
    public void OnPointerEnter(PointerEventData eventData)
    {   
        buildingManager.SelectBuilding(null);
        customCursor.DestroyPreview();
    }

    
}
