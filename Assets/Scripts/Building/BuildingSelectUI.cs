using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSelectUI : MonoBehaviour
{
    [SerializeField] public BuildingTypeList buildingTypeList;
    [SerializeField] private Transform btnTemplate;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CustomCursor customCursor;


    private Dictionary<BuildingType, Transform> btnTransformDictionary;




    private void Awake()
    {
        // btnTemplate.gameObject.SetActive(false);

        // btnTransformDictionary = new Dictionary<BuildingType, Transform>();

        // int index = 0;

        // foreach (BuildingType buildingType in buildingTypeList.list) {
        //     Transform btnTransform = Instantiate(btnTemplate, transform);
        //     btnTransform.gameObject.SetActive(true);
        //     btnTransform.Find("ItemIcon").GetComponent<Image>().sprite = buildingType.sprite;
        //     btnTransform.Find("Bottom/GoldText").GetComponent<TextMeshProUGUI>().text = buildingType.cost.ToString();
        //     btnTransform.GetComponent<Button>().onClick.AddListener(() => {
        //         buildingManager.SelectBuilding(buildingType);
        //         customCursor.SelectObject(buildingType);
        //         SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);


        //     });

        //     btnTransformDictionary[buildingType] = btnTransform;
        //     index++;


        // }
    }

    private void Start()
    {
        btnTemplate.gameObject.SetActive(false);

        btnTransformDictionary = new Dictionary<BuildingType, Transform>();

        int index = 0;

        foreach (BuildingType buildingType in buildingTypeList.list)
        {
            Transform btnTransform = Instantiate(btnTemplate, transform);
            btnTransform.gameObject.SetActive(true);
            btnTransform.Find("ItemIcon").GetComponent<Image>().sprite = buildingType.sprite;
            btnTransform.Find("Bottom/GoldText").GetComponent<TextMeshProUGUI>().text = buildingType.cost.ToString();
            btnTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                buildingManager.SelectBuilding(buildingType);
                customCursor.SelectObject(buildingType);
                SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);


            });

            btnTransformDictionary[buildingType] = btnTransform;
            index++;


        }


        BuildingManager.BuildingSelectionChanged += BuildingSelectionChanged;
        CustomCursor.BuildingPlaced += BuildingPlaced;
    }

    private void OnDestroy()
    {
        BuildingManager.BuildingSelectionChanged -= BuildingSelectionChanged;
        CustomCursor.BuildingPlaced -= BuildingPlaced;
    }

    private void BuildingPlaced()
    {

        ChangeBuildingSelection();

    }

    private void Update()
    {
        foreach (BuildingType buildingType in btnTransformDictionary.Keys)
        {
            Transform btnTransform = btnTransformDictionary[buildingType];
            Transform disableImage = btnTransform.Find("ButtonDisable");
            Button button = btnTransform.GetComponent<Button>();
            if (gameManager.GetCurrentCoin() < buildingType.cost)
            {
                button.interactable = false;
                disableImage.gameObject.SetActive(true);
            }
            else
            {
                button.interactable = true;
                disableImage.gameObject.SetActive(false);
            }
        }
    }

    private void BuildingSelectionChanged()
    {
        ChangeBuildingSelection();


    }

    private void ChangeBuildingSelection()
    {
        foreach (BuildingType buildingType in btnTransformDictionary.Keys)
        {
            Transform btnTransform = btnTransformDictionary[buildingType];
            btnTransform.Find("Focus").gameObject.SetActive(false);
        }

        BuildingType currentSelection = buildingManager.GetCurrentSelection();
        //Debug.Log(buildingManager.GetCurrentSelection());

        if (currentSelection == null)
        {
            foreach (BuildingType buildingType in btnTransformDictionary.Keys)
            {
                Transform btnTransform = btnTransformDictionary[buildingType];
                btnTransform.Find("Focus").gameObject.SetActive(false);
            }
        }
        else
        {
            btnTransformDictionary[currentSelection].Find("Focus").gameObject.SetActive(true);
        }



    }

    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     buildingManager.SelectBuilding(null);
    //     customCursor.DestroyPreview();
    // }


}
