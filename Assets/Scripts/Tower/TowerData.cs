
using UnityEngine;
using UnityEngine.UI;

public class TowerData : MonoBehaviour
{
    [SerializeField] private BuildingType buildingType;
    [SerializeField] private GameObject upgradeEffectPrefab;
    private GameObject upgradeEffect;
    private ParticleSystem upgradeEffectParticles;
    private Animator animator;

    private float attackSpeed, attackRadius;
    private int attackDamage; 
    private string description, nameString;
    private Sprite sprite;
    private int attackUpgradeCost;
    private int speedUpgradeCost;
    private int startAttackUpgradeCost;
    private int startSpeedUpgradeCost;
    private int attackUpgradeIncrement;
    private int speedUpgradeIncrement;
    
    
    private void Awake() {
        attackSpeed = buildingType.attackSpeed;
        attackRadius = buildingType.attackRadius;
        attackDamage = buildingType.attackDamage;
        nameString = buildingType.nameString;
        description = buildingType.description;
        sprite = buildingType.sprite;
        attackUpgradeCost = buildingType.startAttackUpgradeCost;
        speedUpgradeCost = buildingType.startSpeedUpgradeCost;
    }

    

    private void OnEnable() {
        if (upgradeEffect == null) {
            upgradeEffect = Instantiate(upgradeEffectPrefab, transform.position, Quaternion.identity);
            upgradeEffect.transform.SetParent(transform, true);
        }
        upgradeEffectParticles = upgradeEffect.GetComponent<ParticleSystem>();
        upgradeEffectParticles.Stop();
    }

    private void Start() {
        animator = GetComponent<Animator>();
        upgradeEffectParticles.Stop();
    }

    public float GetAttackRadius() {
        return attackRadius;
    }

    public int GetAttackDamage() {
        return attackDamage;
    }

    public float GetAttackSpeed () {
        return attackSpeed;
    }

    public Sprite GetSprite() {
        return sprite;
    }

    public string GetDescription() {
        return description;
    }

    public string GetNameString() {
        return nameString;
    }

    public int GetUpgradeAtkCost() {
        return attackUpgradeCost;
    }

    public int GetUpgradeSpeedCost() {
        return speedUpgradeCost;
    }

    private void IncreaseAtkUpCost() {
        attackUpgradeCost += buildingType.attackUpgradeCost;
        
    }

    private void IncreaseSpdUpCost() {
        speedUpgradeCost += buildingType.speedUpgradeCost;
    }

    public void IncreaseAttackDamage () {
        attackDamage += 1;
        IncreaseAtkUpCost();
    }

    public void IncreaseAttackSpeed () {
        attackSpeed += 0.2f;
        animator.SetFloat("attackSpeed", attackSpeed);
        IncreaseSpdUpCost();
    }

    public void PlayUpgradeEffect() {
        upgradeEffectParticles.Play();
    }
}
