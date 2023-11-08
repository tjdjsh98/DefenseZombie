using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "Data", menuName = "SetupData", order = 5)]
public class SetupData : ScriptableObject
{
    [field: SerializeField] public ItemName HatItem { get; set; }
    [field: SerializeField] public ItemName WeaponItem { get; set; }
}
