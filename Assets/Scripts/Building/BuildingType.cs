using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building")]
public class BuildingType : ScriptableObject
{
    public string nameString;

    [TextArea(1,5)]
    public string description;
    public Transform prefab;
    public Transform prefabPreview;
    public Sprite sprite;
    public int cost;
    public int attackDamage;
    public float attackSpeed;
    public float attackRadius;
    public int startAttackUpgradeCost;
    public int startSpeedUpgradeCost;
    public int attackUpgradeCost;
    public int speedUpgradeCost;
}
