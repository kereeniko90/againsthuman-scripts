using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{   
    
    
    [SerializeField] BuildingSelectUI buildingSelectUI;
    
    private BuildingType currentSelection;

    public static event Action BuildingSelectionChanged;

    private void Update() {
        
    }

    public void SelectBuilding(BuildingType buildingType) {

        
        currentSelection = buildingType;
        BuildingSelectionChanged?.Invoke();
        
    }

    public BuildingType GetCurrentSelection() {
        return currentSelection;
    }

    public void SetCurrentBuildingToNull () {
        currentSelection = null;
    }

    
}
