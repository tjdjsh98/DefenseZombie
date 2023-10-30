using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[System.Serializable]
public class BuildingBlueprint
{
    [SerializeField]List<BlueprintItem> _blueprintItmeList = new List<BlueprintItem>();
    public List<BlueprintItem> BlueprintItmeList => _blueprintItmeList;
}

[System.Serializable]
public class BlueprintItem
{
    public ItemName name;
    public int requireCount;
    public int currentCount;

    public void AddCount()
    {
        currentCount++;
    }
}
