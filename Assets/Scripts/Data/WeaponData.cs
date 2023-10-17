using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName ="Data", menuName ="WeaponData",order =1)]
public class WeaponData : ScriptableObject
{
    [field:SerializeField]public WeaponName WeaponName { set; get; }
    [field: SerializeField] public Sprite ThumbnailSprite { set; get; }
    [field: SerializeField] public Sprite EquipSprite { set; get; }
    [field: SerializeField] public AnimationClip AttackAnimationClip { set; get; }
}
