using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public static int PlayerLayerMask = LayerMask.GetMask("Player");
    public static int EnemyLayerMask = LayerMask.GetMask("Enemy");
    public static int CharacterLayerMask = PlayerLayerMask | EnemyLayerMask;
    public static int BuildingLayerMask = LayerMask.GetMask("BothPassableBuilding") |
        LayerMask.GetMask("OnlyPlayerPassableBuilding") |
        LayerMask.GetMask("OnlyEnemyPassableBuilding") |
        LayerMask.GetMask("BothUnpassableBuilding");

    public static int GroundLayer = LayerMask.GetMask("Ground");
    public enum AttacKShape
    {
        Rectagle,
        Circle,
        Raycast
    }

    public enum BuildingName
    { 
        CommandCenter = 0,
        Barricade = 1,
        Rock = 2,
        Cannon = 3,
        StepBox = 4,
        Core = 5,
        GrassTile = 6,
        GroundTile = 7,
        Tower = 8,
    }
    public enum CharacterName
    {
        PistalCharacter,
        HammerCharacter,
        Helper,
        MiniBear,
        Bear,
        BigBear,
        SpannerCharacter,
        Zombie,
        ShieldMiniBear,
        Horriy,
        CustomCharacter,
        Bee,
        CustomEnemy
    }

    public enum WeaponName
    {
        None,
        Gun,
        Sword,
        Spear
    }

    public enum ItemName
    {
        None,
        Wood,
        Stone,
        Iron,
        Gun,
        Sword,
        Spear,
        BeeHive
    }
    public enum ItemType
    {
        Etc,
        Equipment
    }

    public enum CharacterParts
    {
        Head,
        Eyes,
        Body,
        Legs,
        FrontHand,
        BehindHand,
        FrontWeapon,
        BehindWeapon,
    }

    public enum CharacterTag
    {
        Player,
        Enemy
    }

    public enum UIName
    {
        Level,
        Commander,
        Item,
        Build
    }
    public enum EffectName
    {
        None,
        Normal,
        Dash,
    }
}


public interface ICharacterOption
{
    public void Init();
}

