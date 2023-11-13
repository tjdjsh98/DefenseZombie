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
    public int HatItemId => _hatItemId;
    int _bodyItemId;
    public int BodyItemId => _bodyItemId;
    int _legsItemId;
    public int LegsItemId => _legsItemId;
    int _handItemId;
    public int HandItemId => _handItemId;
    int _weaponId;
    public int WeaponId => _weaponId;

    public Action EquipmentChanged { get; set; }

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
        if (item == null) return false;
        if(item.ItemData.ItemType != ItemType.Equipment) return false;

        if (item.ItemData.EquipmentData == null)
        {
            if (EquipWeapon(item))
            {
                return true;
            }
        }
        else
        {
            if (EquipOther(item))
            {
                return true;
            }
        }

        if (!Client.Instance.IsSingle && Client.Instance.IsMain)
        {
            Client.Instance.SendCharacterInfo(_customCharacter);
        }


        return false;
    }
    bool EquipWeapon(Item item)
    {
        if (item == null) return false;

        string ItemName = item.ItemData.ItemName.ToString();

        WeaponName name = WeaponName.None;
        Enum.TryParse(ItemName, true, out name);

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

        item.IsGraped = true;
        item.Hide();

        _weaponId = item.ItemId;

        EquipmentChanged?.Invoke();


        return true;
    }
    public bool TakeOffWeapon(bool putDown = true)
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

        Item item = Manager.Item.GetItem(_weaponId);

        item.Show();
        item.ReleaseItem(_customCharacter, putDown);
        _weaponId = 0;

        EquipmentChanged?.Invoke();

        return true;
    }

    public bool EquipOther(Item item)
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

            item.Hide();
            item.IsGraped = true;

        }
        else
        {
            return false;
        }
        EquipmentChanged?.Invoke();

        return true;
    }

    // 해당파츠의 아이템번호를 반환합니다.
    public int TakeOffOther(CharacterParts part, bool putDown = true)
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

        Item item = Manager.Item.GetItem(returnId);

        if (item != null)
        {
            item.Show();
            item.ReleaseItem(_customCharacter, putDown);
        }
        EquipmentChanged?.Invoke();

        return returnId;
    }
    public void DataSerialize()
    {
        Util.WriteSerializedData(_hatItemId);
        Util.WriteSerializedData(_bodyItemId);
        Util.WriteSerializedData(_legsItemId);
        Util.WriteSerializedData(_handItemId);
        Util.WriteSerializedData(_weaponId);
    }

    public void DataDeserialize()
    {
        int hatItemId = Util.ReadSerializedDataToInt();
        int bodyItemId = Util.ReadSerializedDataToInt();
        int legsItemId = Util.ReadSerializedDataToInt();
        int handItemId = Util.ReadSerializedDataToInt();
        int weaponId = Util.ReadSerializedDataToInt();

        if(_hatItemId != hatItemId)
        {
            if (hatItemId != 0)
                EquipOther(Manager.Item.GetItem(hatItemId));
            else
                TakeOffOther(CharacterParts.Hat);
        }
        if (_bodyItemId != bodyItemId)
        {
            if (bodyItemId != 0)
                EquipOther(Manager.Item.GetItem(bodyItemId));
            else
                TakeOffOther(CharacterParts.Body);
        }
        if (_legsItemId != legsItemId)
        {
            if(legsItemId!= 0)
                EquipOther(Manager.Item.GetItem(legsItemId));
            else
                TakeOffOther(CharacterParts.Legs);
        }
        if (_handItemId != handItemId)
        {
            if (handItemId != 0)
                EquipOther(Manager.Item.GetItem(handItemId));
            else
                TakeOffOther(CharacterParts.FrontHand);
        }
        if (_weaponId != weaponId)
        {
            if (weaponId != 0)
            {
                EquipItem(Manager.Item.GetItem(weaponId));
            }
            else
                TakeOffWeapon();
        }
    }

}
