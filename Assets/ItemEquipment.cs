using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipment : MonoBehaviour, IItemOption
{
    Item _item;

    int _rank = 0;
    public int Rank =>_rank;
    int _defense;
    int _addedDefense;
    public int AddedDefense => _addedDefense;
    [field: SerializeField] public int Defense => _defense + _addedDefense;
    int _hp;
    int _addedHp;
    public int AddedHp => _addedHp;
    [field: SerializeField] public int Hp => _addedHp + _hp;

    float _speed;
    float _addedSpeed;
    public float AddedSpeed => _addedSpeed;
    [field: SerializeField] public float Speed => _addedSpeed + _speed;

    int _attack;
    int _addedAttack;
    public int AddedAttack => _addedAttack;
    [field: SerializeField] public float Attack => _attack + _addedAttack;
    public bool IsDone { get; set; }

    public void Init()
    {
        _item = GetComponent<Item>();

        if (_item.ItemData.EquipmentData != null)
        {
            _defense = _item.ItemData.EquipmentData.AddDefense;
            _hp = _item.ItemData.EquipmentData.AddHp;
            _speed = _item.ItemData.EquipmentData.AddSpeed;
        }
    }

    public void AddSpeed(float add)
    {
        _rank++;
        _addedSpeed += add;
        Client.Instance.SendItemInfo(_item);
    }

    public void AddHp(int hp)
    {
        _rank++;
        _addedHp += hp;
        Client.Instance.SendItemInfo(_item);
    }

    public void AddDefense(int defense)
    {
        _rank++;
        _addedDefense += defense;
        Client.Instance.SendItemInfo(_item);
    }

    public void AddAttack(int attack)
    {
        _rank++;
        _addedAttack += attack;
        Client.Instance.SendItemInfo(_item);
    }



    public void SerializeData()
    {
        Util.WriteSerializedData(_rank);
        Util.WriteSerializedData(_attack);
        Util.WriteSerializedData(_addedAttack);
        Util.WriteSerializedData(_defense);
        Util.WriteSerializedData(_addedDefense);
        Util.WriteSerializedData(_speed);
        Util.WriteSerializedData(_addedSpeed);
    }
    public void DeserializeData()
    {
        _rank = Util.ReadSerializedDataToInt();
        _attack = Util.ReadSerializedDataToInt();
        _addedAttack = Util.ReadSerializedDataToInt();
        _defense = Util.ReadSerializedDataToInt();
        _addedDefense = Util.ReadSerializedDataToInt();
        _speed = Util.ReadSerializedDataToFloat();
        _addedSpeed = Util.ReadSerializedDataToFloat();
    }
    public void SerializeControlData()
    {
    }
    public void DeserializeControlData()
    {
    }
}
