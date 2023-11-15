using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipment : MonoBehaviour, IItemOption
{
    Item _item;

    int _defense;
    int _addedDefense;
    [field: SerializeField] public int AddDefense => _defense + _addedDefense;
    int _hp;
    int _addedHp;
    [field: SerializeField] public int AddHp => _addedHp + _hp;
    float _speed;
    float _addedSpeed;
    [field: SerializeField] public float AddSpeed => _addedSpeed + _speed;

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
