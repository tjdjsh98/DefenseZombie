using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "Data", menuName = "ItemData", order = 0)]

public class ItemData : ScriptableObject
{
    [field: SerializeField] public ItemName ItemName { set; get; }
    [field: SerializeField] public Sprite ItemSprite { set; get; }
    [field: SerializeField] public ItemType ItemType { set; get; }
    [field: SerializeField] public GameObject Origin{ set; get; }
}
