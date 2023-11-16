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
    [field: SerializeField] public int Defense => _defense + _addedDefense;
    int _hp;
    int _addedHp;
    [field: SerializeField] public int Hp => _addedHp + _hp;
    float _speed;
    float _addedSpeed;
    [field: SerializeField] public float Speed => _addedSpeed + _speed;

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
    }

    public void AddHp(int hp)
    {
        _rank++;
        _addedHp += hp;
    }

    public void AddDefense(int defense)
    {
        _rank++;
        _addedDefense += defense;
    }




    public void DeserializeControlData()
    {
    }

    public void DeserializeData()
    {
    }


    public void SerializeControlData()
    {
    }

    public void SerializeData()
    {
    }

}
