using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "Data", menuName = "ItemBlueprint", order = 0)]
public class ItemBlueprintData : ScriptableObject
{
    [field:SerializeField]public ItemName ResultItemName { get; set; }
    [field:SerializeField]public List<BlueprintItem> BlueprintItemList { get; set; }
    
}
