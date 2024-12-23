using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSelection : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RectTransform towerStatCanvas;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private TextMeshProUGUI towerDescriptionText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button atkUpgradeButton;
    [SerializeField] private Button spdUpgradeButton;
    [SerializeField] private CustomCursor customCursor;


    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    private Outline highlightOutline;
    private Outline selectionOutline;


    private TowerStats towerStats;



    private void Start()
    {
        towerStats = towerStatCanvas.GetComponent<TowerStats>();
    }

    void Update()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit, 5000, layerMask))
        {
            Transform hitTransform = raycastHit.transform;


            if (highlight != hitTransform)
            {
                ResetHighlight();

                highlight = hitTransform;
                highlightOutline = highlight.GetComponent<Outline>();

                if (highlightOutline != null && highlight != selection)
                {
                    highlightOutline.enabled = true;
                    highlightOutline.OutlineColor = Color.yellow;
                }
            }
        }
        else
        {
            ResetHighlight();
        }

        if (Input.GetMouseButtonDown(0))
        {


            if (highlight != null && !customCursor.GetPlacedDelay())
            {

                if (highlight != selection)
                {
                    StartCoroutine(SwitchSelection(highlight));
                }

                ResetHighlight();
            }
            else
            {
                if (selection != null && !EventSystem.current.IsPointerOverGameObject())
                {
                    selectionOutline.enabled = false;
                    selection = null;
                    towerStats.HideUI();
                }
            }
        }

        if (selection != null)
        {
            TowerData towerData = selection.GetComponent<TowerData>();
            if (gameManager.GetCurrentCoin() < towerData.GetUpgradeAtkCost())
            {
                atkUpgradeButton.interactable = false;


            }
            else
            {
                atkUpgradeButton.interactable = true;

            }

            if (gameManager.GetCurrentCoin() < towerData.GetUpgradeSpeedCost())
            {
                spdUpgradeButton.interactable = false;

            }
            else
            {
                spdUpgradeButton.interactable = true;

            }
        }





    }

    private void ResetHighlight()
    {
        if (highlight != null && highlightOutline != null && highlight != selection)
        {
            highlightOutline.enabled = false;
            highlightOutline.OutlineColor = Color.yellow;
        }

        highlight = null;
        highlightOutline = null;
    }

    private IEnumerator SwitchSelection(Transform newSelection)
    {
        // Hide current selection UI
        if (selection != null)
        {
            selectionOutline.OutlineColor = Color.yellow;
            SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);
            selectionOutline.enabled = false;
            towerStats.HideUI();
        }

        // Small delay to ensure UI has finished hiding
        yield return new WaitForSeconds(0.1f);

        // Update selection and show new UI
        selection = newSelection;
        selectionOutline = selection.GetComponent<Outline>();


        UpdateTowerStat();


        if (selectionOutline != null)
        {
            selectionOutline.enabled = true;
            SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);
            selectionOutline.OutlineColor = Color.green;
            towerStats.ShowUI();


        }
    }

    private void UpdateTowerStat()
    {
        TowerData towerData = selection.GetComponent<TowerData>();


        iconImage.sprite = towerData.GetSprite();
        towerNameText.text = towerData.GetNameString();
        towerDescriptionText.text = towerData.GetDescription();
        towerStats.UpdateAttackText(towerData.GetAttackDamage());
        towerStats.UpdateSpeedText(towerData.GetAttackSpeed());
        towerStats.UpdateAttackCost(towerData.GetUpgradeAtkCost());
        towerStats.UpdateSpeedCost(towerData.GetUpgradeSpeedCost());

    }

    public void UpgradeAttackDamage()
    {
        if (selection != null)
        {
            TowerData towerData = selection.GetComponent<TowerData>();
            int upgradeCost = towerData.GetUpgradeAtkCost();
            SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);

            if (gameManager.GetCurrentCoin() >= upgradeCost)
            {
                gameManager.ReduceCoin(towerData.GetUpgradeAtkCost());
                towerData.IncreaseAttackDamage();
                towerData.PlayUpgradeEffect();
                SoundManager.Instance.PlaySound(SoundManager.Sound.UpgradeSound);
                towerStats.UpdateAttackText(towerData.GetAttackDamage());
                towerStats.UpdateAttackCost(towerData.GetUpgradeAtkCost());
                
                
                

                atkUpgradeButton.interactable = gameManager.GetCurrentCoin() >= towerData.GetUpgradeAtkCost();
            }

        }

    }

    public void UpgradeAttackSpeed()
    {
        if (selection != null)
        {
            TowerData towerData = selection.GetComponent<TowerData>();
            int upgradeCost = towerData.GetUpgradeSpeedCost();
            SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);

            if (gameManager.GetCurrentCoin() >= upgradeCost)
            {
                gameManager.ReduceCoin(towerData.GetUpgradeSpeedCost());
                towerData.IncreaseAttackSpeed();
                towerData.PlayUpgradeEffect();
                SoundManager.Instance.PlaySound(SoundManager.Sound.UpgradeSound);
                towerStats.UpdateSpeedText(towerData.GetAttackSpeed());
                towerStats.UpdateSpeedCost(towerData.GetUpgradeSpeedCost());
                
                
                

                spdUpgradeButton.interactable = gameManager.GetCurrentCoin() >= towerData.GetUpgradeSpeedCost();
            }

        }

    }

    public void CloseButton()
    {
        selectionOutline.enabled = false;
        SoundManager.Instance.PlaySound(SoundManager.Sound.Tik);
        selection = null;
        towerStats.HideUI();
    }

}
