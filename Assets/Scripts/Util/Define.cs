using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public static int PlayerLayerMask = LayerMask.GetMask("Player");
    public static int PlayerLayer = LayerMask.NameToLayer("Player");
    public static int EnemyLayerMask = LayerMask.GetMask("Enemy");
    public static int EnemyLayer = LayerMask.NameToLayer("Enemy");
    public static int CharacterLayerMask = PlayerLayerMask | EnemyLayerMask;
    public static int BuildingLayerMask = LayerMask.GetMask("BothPassableBuilding") |
        LayerMask.GetMask("OnlyPlayerPassableBuilding") |
        LayerMask.GetMask("OnlyEnemyPassableBuilding") |
        LayerMask.GetMask("BothUnpassableBuilding");
    public static int GroundLayerMask = LayerMask.GetMask("Ground");
    public static int UnconstructedBuildingLayerMask = LayerMask.GetMask("UnconstrctedBuilding");
    public static int UnconstructedBuildingLayer = LayerMask.NameToLayer("UnconstrctedBuilding");
    public static int ItemLayerMask = LayerMask.GetMask("Item");

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
        Tree= 9,
        Smithy = 10,
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
        Spear,
        Handgun,
        ZeusSpear,
        Bow
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
        BeeHive,
        Supplies,
        Handgun,
        ZeusSpear,
        WorkerHat,
        Bow
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
        Hat
    }

    public enum EquipmentName
    {
        None,
        Normal,
        Worker,
        Solider,
        Bear,
    }

    public enum CharacterTag
    {
        Player,
        Enemy,
        Building
    }

    public enum UIName
    {
        Level,
        Commander,
        Item,
        Build,
        Smithy,
        Debug,
        GameOver,
        Equipment
    }
    public enum EffectName
    {
        None,
        Normal,
        Dash,
        Dust,
        Thunder
    }
    public enum ProjectileType
    {
        Linear,
        Arrow
    }
}

