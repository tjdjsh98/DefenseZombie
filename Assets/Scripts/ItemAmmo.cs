using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAmmo : MonoBehaviour, IItemOption
{
    public bool IsDone { get; set; }

    [field:SerializeField]public ItemData RequireItem { get; set; }
    [SerializeField] int _currnetAmmo;
    public int currentAmmo
    {
        get { return _currnetAmmo; }
        set { _currnetAmmo = value; AmmoChangedHandler?.Invoke(); }
    }

    [field:SerializeField] public int MaxAmmon { get; set; }

    public Action AmmoChangedHandler;

    public void Init()
    {
        IsDone = true;
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
