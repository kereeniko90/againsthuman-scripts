using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypeList", menuName = "Scriptable Objects/BuildingTypeList")]
public class BuildingTypeList : ScriptableObject
{
    public List<BuildingType> list;
}
