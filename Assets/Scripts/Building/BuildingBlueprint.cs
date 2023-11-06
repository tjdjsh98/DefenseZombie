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
    public BlueprintItem(ItemName name,int requireCount, int currentCount)
    {
        this.name = name;
        this.requireCount = requireCount;
        this.currentCount = currentCount;
    }

    public ItemName name;
    public List<int> itemIdList =new List<int>();
    public int requireCount;
    public int currentCount;

    public void AddCount(int id)
    {
        itemIdList.Add(id);
        currentCount++;
    }
}
