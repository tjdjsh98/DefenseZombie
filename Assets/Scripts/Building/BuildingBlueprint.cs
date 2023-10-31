using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[System.Serializable]
public class BuildingBlueprint
{
    [SerializeField]List<BlueprintItem> _blueprintItmeList = new List<BlueprintItem>();
    public List<BlueprintItem> BlueprintItemList => _blueprintItmeList;
}

[System.Serializable]
public class BlueprintItem
{
    public BlueprintItem(BlueprintItem blueprintItem)
    {
        name = blueprintItem.name;
        requireCount = blueprintItem.requireCount;
        currentCount = blueprintItem.currentCount;
    }
    public ItemName name;
    public int requireCount;
    public int currentCount;

    public void AddCount()
    {
        currentCount++;
    }
}
