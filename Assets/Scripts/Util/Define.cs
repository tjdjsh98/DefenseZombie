using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
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
        Core
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
        MiniCharacter
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
        Spear
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
