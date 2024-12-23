using System;
using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] BuildingTypeList buildingTypeList;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] private GameObject buildingGhost;
    [SerializeField] private GameObject radiusDisplay;
    [SerializeField] LayerMask layerMask;
    [SerializeField] private float lerpSpeed = 5f;

    [SerializeField] private Material placeableBlue;
    [SerializeField] private Material unplaceableRed;
    [SerializeField] private GameObject rotateMessage;

    public static event Action BuildingPlaced;

    private SkinnedMeshRenderer previewRenderer;
    private Vector3 placeLocation;
    private Quaternion placeRotation;
    private Vector3 tilePosition;
    private RaycastHit hit;
    private Transform previewObject;

    private Camera mainCamera;

    private bool isInPlacementMode = false;
    private bool isHittingTile = false;

    private bool isPlaceable = false;
    private bool placedDelay = false;
    
    private float placedDelayRestTime = 1f;
    private Animator rotateMessageAnimator;

    private void Start()
    {
        mainCamera = Camera.main;
        rotateMessageAnimator = rotateMessage.GetComponent<Animator>();


    }



    private void Update()
    {   
        
        Vector3 updateTilePosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);

        if (isHittingTile)
        {
            buildingGhost.transform.position = Vector3.Lerp(buildingGhost.transform.position, updateTilePosition, Time.deltaTime * lerpSpeed);
            

            if (previewObject != null)
            {
                previewObject.transform.position = Vector3.Lerp(previewObject.transform.position, updateTilePosition, Time.deltaTime * lerpSpeed);
            } else {
                radiusDisplay.gameObject.SetActive(false);
            }

            placeLocation = updateTilePosition;

            if (isInPlacementMode)
            {
                
                ChangePreviewColor();
                ChangePreviewRotation();
                PlaceSelectedBuilding();
                rotateMessageAnimator.SetBool("show", true);

            } else {
                rotateMessageAnimator.SetBool("show", false);
            }
            

        }
        else
        {
            buildingGhost.transform.position = updateTilePosition;
            
            
                
            
            

            if (previewObject != null)
            {
                previewObject.transform.position = updateTilePosition;
                previewRenderer.material = unplaceableRed;
            }
        }

        if (placedDelay) {
            StartCoroutine(ResetPlacedDelay());
        }



    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 5000, layerMask) && !EventSystem.current.IsPointerOverGameObject())
        {

            tilePosition = hit.transform.position;
            Waypoint waypoint = hit.transform.GetComponent<Waypoint>();
            isPlaceable = waypoint.IsPlaceable();
            isHittingTile = true;
            //isInPlacementMode = true;
            radiusDisplay.gameObject.SetActive(true);

            
            
            
            

        }
        else
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isHittingTile = false;
            isPlaceable = false;

            tilePosition = mousePos;
            radiusDisplay.gameObject.SetActive(false);
        }


    }

    private void ChangePreviewColor()
    {
        if (previewObject != null)
        {   
            BuildingType currentBuildingType = buildingManager.GetCurrentSelection();
            float areaRadius = currentBuildingType.attackRadius;
            radiusDisplay.transform.localScale = new Vector3(areaRadius *2, 0, areaRadius * 2);
            
            
            if (isPlaceable)
            {
                previewRenderer.material = placeableBlue;
            }
            else
            {
                previewRenderer.material = unplaceableRed;
            }
        }
    }

    private void ChangePreviewRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            previewObject.transform.Rotate(0, 90, 0);
            SoundManager.Instance.PlaySound(SoundManager.Sound.Rotate);
            placeRotation = previewObject.transform.rotation;
        }
    }

    private void PlaceSelectedBuilding()
    {
        if (Input.GetMouseButtonDown(0))
        {

            BuildingType currentBuildingType = buildingManager.GetCurrentSelection();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Waypoint waypoint;
            if (Physics.Raycast(ray, out hit, 5000, layerMask) && !EventSystem.current.IsPointerOverGameObject()) {
                waypoint = hit.transform.GetComponent<Waypoint>();
            } else {
                waypoint = null;
            }

            if (currentBuildingType != null && isPlaceable && waypoint != null)
            {

                PlaceObject(currentBuildingType, placeLocation, placeRotation);
                
                gameManager.ReduceCoin(currentBuildingType.cost);
                waypoint.DisablePlaceable();
                isInPlacementMode = false;
                placedDelay = true;
                
            }

        }
    }




    public void SelectObject(BuildingType buildingType)
    {   
        if (previewObject != null) {
            Destroy(previewObject.gameObject);
            previewObject = null;
        }
        previewObject = Instantiate(buildingType.prefabPreview, tilePosition, transform.rotation);
        previewRenderer = previewObject.GetComponentInChildren<SkinnedMeshRenderer>();
        isInPlacementMode = true;

    }

    public void DestroyPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject.gameObject);
            previewObject = null;
        }
        isInPlacementMode = false;
    }

    public void PlaceObject(BuildingType buildingType, Vector3 location, Quaternion rotation)
    {
        buildingManager.SetCurrentBuildingToNull();
        BuildingPlaced?.Invoke();
        Destroy(previewObject.gameObject);
        previewObject = null;
        
        Instantiate(buildingType.prefab, placeLocation, rotation);
        SoundManager.Instance.PlaySound(SoundManager.Sound.Boop);

    }

    public bool IsInPlacementMode() {
        return isInPlacementMode;
    }

    public bool GetPlacedDelay() {
        return placedDelay;
    }

    private IEnumerator ResetPlacedDelay()
    {
        float elapsedTime = 0f;
        while (elapsedTime < placedDelayRestTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        placedDelay = false;
    }


}
