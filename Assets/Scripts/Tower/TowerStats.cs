using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerStats : MonoBehaviour
{
    

    [SerializeField] private TextMeshProUGUI currentAttackText;
    [SerializeField] private TextMeshProUGUI currentSpeedText;
    [SerializeField] private TextMeshProUGUI attackUpgradeCostText;
    [SerializeField] private TextMeshProUGUI speedUpgradeCostText;
    
    [SerializeField] private GameManager gameManager;

    

    private Animator animator;
    private bool appeared = false;
    

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        
        
    }


    public void HideUI() {
        animator.SetBool("appear", false);
        appeared = false;
    }

    public void ShowUI() {
        animator.SetBool("appear", true);
        appeared = true;
    }

    public bool GetUIStatus() {
        return appeared;
    }

    public void UpdateAttackText(int currentAttack) {
        currentAttackText.text = "Attack Damage: " + currentAttack;
    }

    public void UpdateSpeedText(float currentSpeed) {
        currentSpeedText.text = "Attack Speed: " + currentSpeed.ToString("F1");
    }

    public void UpdateAttackCost(int cost) {
        attackUpgradeCostText.text = cost.ToString();
    }

    public void UpdateSpeedCost(int cost) {
        speedUpgradeCostText.text = cost.ToString();
    }

    

}
