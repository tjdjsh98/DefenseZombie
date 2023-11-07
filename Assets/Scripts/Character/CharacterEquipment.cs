using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CharacterEquipment : MonoBehaviour, ICharacterOption
{
    CustomCharacter _customCharacter;
    SpriteChanager _spriteChanger;
    Animator _animator;

    WeaponName _equipWeaponName;
    public WeaponName EquipWeaponName =>_equipWeaponName;

    EquipmentName _equipHatName;
    EquipmentName _equipBodyName;
    EquipmentName _equipLegsName;
    EquipmentName _equipFrontHandName;
    EquipmentName _equipBehindHandName;

    int _hatItemId;
    int _bodyItemId;
    int _legsItemId;
    int _handItemId;

    public bool IsDone { set; get; }
    public void Init()
    {
        _customCharacter = GetComponent<CustomCharacter>();
        _spriteChanger = GetComponent<SpriteChanager>();
        _animator = GetComponent<Animator>();
        IsDone = true;
    }

    public bool EquipItem(Item item)
    {
        if(item.ItemData.ItemType != ItemType.Equipment) return false;

        if (item.ItemData.EquipmentData == null)
        {
            string ItemName = item.ItemData.ItemName.ToString();

            WeaponName weaponName = WeaponName.None;
            Enum.TryParse(ItemName, true, out weaponName);

            if (EquipWeapon(weaponName))
            {
                item.Hide();
                _customCharacter.SetHoldingItemId(item.ItemId);
                item.IsGraped = true;

                return true;
            }
        }
        else
        {
            if (EquipOther(item))
            {
                item.Hide();
                item.IsGraped = true;

                return true;
            }
        }


        return false;
    }
    bool EquipWeapon(WeaponName name)
    {
        _equipWeaponName = name;

        WeaponData data = Manager.Data.GetWeaponData(name);

        if (data == null) return false;


        if (data.IsFrontWeapon)
            _spriteChanger.ChangeSprite(CharacterParts.FrontWeapon, data.WeaponSpriteLibrary);
        else
            _spriteChanger.ChangeSprite(CharacterParts.BehindWeapon, data.WeaponSpriteLibrary);

        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        if (data.AttackAnimationClip != null)
        {
            if (data.IsFrontWeapon)
                myOverrideController["CustomCharacterFrontHandAttack"] = data.AttackAnimationClip;
            else
                myOverrideController["CustomCharacterBehindHandAttack"] = data.AttackAnimationClip;
        }

        if (data.IsFrontWeapon)
            _customCharacter.SetEnableFrontHandRotate();
        else
            _customCharacter.SetEnableBehindHandRotate();

        return true;
    }
    public bool TakeOffWeapon()
    {
        if (_equipWeaponName == WeaponName.None) return false;

        WeaponData data = Manager.Data.GetWeaponData(WeaponName.None);

        _spriteChanger.ChangeDefaultSprite(CharacterParts.FrontWeapon);
        _spriteChanger.ChangeDefaultSprite(CharacterParts.BehindWeapon);

        RuntimeAnimatorController myController = _animator.runtimeAnimatorController;

        AnimatorOverrideController myOverrideController = myController as AnimatorOverrideController;

        if (data.IsFrontWeapon)
            myOverrideController["CustomCharacterFrontHandAttack"] = data.AttackAnimationClip;
        else
            myOverrideController["CustomCharacterBehindHandAttack"] = data.AttackAnimationClip;

        _customCharacter.SetDisableFrontHandRotate();
        _customCharacter.SetDisableBehindHandRotate();

        _equipWeaponName = WeaponName.None;

        return true;
    }

    bool EquipOther(Item item)
    {
        if(item == null) return false;

        EquipmentName equipmentName = item.ItemData.EquipmentData.EquipmentName;
        CharacterParts part = item.ItemData.EquipmentData.CharacterParts;

        EquipmentData data = Manager.Data.GetEquipmentData(equipmentName, part);
        if (data == null) return false;

        if (_spriteChanger.ChangeSprite(part, data.SpriteLibraryAsset))
        {
            switch (part)
            {
                case CharacterParts.Body:
                    _equipBodyName = equipmentName;
                    break;
                case CharacterParts.Legs:
                    _equipLegsName = equipmentName;
                    break;
                case CharacterParts.FrontHand:
                    _equipFrontHandName = equipmentName;
                    _handItemId = item.ItemId;
                    break;
                case CharacterParts.BehindHand:
                    _equipBehindHandName = equipmentName;
                    _handItemId = item.ItemId;
                    break;
                case CharacterParts.Hat:
                    _equipHatName = equipmentName;
                    _hatItemId = item.ItemId;
                    break;
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    // 해당파츠의 아이템번호를 반환합니다.
    int TakeOffOther(CharacterParts part)
    {
        int returnId = 0;
        switch (part)
        {
            case CharacterParts.Body:
                _equipBodyName = EquipmentName.None;
                returnId = _bodyItemId;
                _bodyItemId = 0;
                
                break;
            case CharacterParts.Legs:
                _equipLegsName = EquipmentName.None;
                returnId = _legsItemId;
                _legsItemId = 0;
                break;
            case CharacterParts.FrontHand:
                _equipFrontHandName = EquipmentName.None;
                returnId = _handItemId;
                _handItemId = 0;
                _spriteChanger.ChangeDefaultSprite(CharacterParts.BehindHand);
                break;
            case CharacterParts.BehindHand:
                _equipBehindHandName = EquipmentName.None;
                returnId = _handItemId;
                _handItemId = 0;
                _spriteChanger.ChangeDefaultSprite(CharacterParts.FrontHand);
                break;
            case CharacterParts.Hat:
                _equipHatName = EquipmentName.None;
                returnId = _hatItemId;
                _hatItemId = 0;
                break;
        }

        _spriteChanger.ChangeDefaultSprite(part);
        return returnId;
    }

    public void DataDeserialize()
    {
    }

    public void DataSerialize()
    {
    }
}
