using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class Core : MonoBehaviour, IBuildingOption
{
    Building _building;

    public void Init()
    {
        _building = GetComponent<Building>();

        _building.DestroyedHandler += OnDestroyed;
    }

    public void DeserializeData()
    {

    }
    public void SerializeData()
    {
        
    }

    void OnDestroyed()
    {
    }

    public void SerializeControlData()
    {
    }

    public void DeserializeControlData()
    {
    }
}
