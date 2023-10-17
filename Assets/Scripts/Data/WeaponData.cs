using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using static Define;

[CreateAssetMenu(fileName ="Data", menuName ="WeaponData",order =1)]
public class WeaponData : ScriptableObject
{
    [field:SerializeField]public WeaponName WeaponName { set; get; }
    [field: SerializeField] public Sprite ThumbnailSprite { set; get; }
    [field: SerializeField] public SpriteLibraryAsset WeaponSpriteLibrary { set; get; }
    [field: SerializeField] public AnimationClip AttackAnimationClip { set; get; }
    [field: SerializeField] public List<Attack> AttackList { set; get; }
}
