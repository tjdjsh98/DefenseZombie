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
        CommandCenter,
        Barricade,
        Rock,
        Cannon,
        StepBox,
        Core,
        GrassTile,
        GroundTile,
        Tower,
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
        MiniCharacter,
        Bee
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
}
