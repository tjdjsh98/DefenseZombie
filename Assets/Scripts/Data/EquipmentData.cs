using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using static Define;

[CreateAssetMenu(fileName = "Data", menuName = "Equipment", order = 0)]
public class EquipmentData : ScriptableObject
{
    [field: SerializeField] public CharacterParts CharacterParts { set; get; }
    [field: SerializeField] public EquipmentName EquipmentName { set; get; }
    [field: SerializeField] public int AddDefense { set; get; }
    [field: SerializeField] public int AddHp { set; get; }
    [field: SerializeField] public float AddSpeed { set; get; }
    [field: SerializeField] public SpriteLibraryAsset SpriteLibraryAsset { set; get; }

}
