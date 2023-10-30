using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DropItemBuildingOption : MonoBehaviour, IBuildingOption
{
    Building _building;

    [SerializeField] List<ItemName> data;
    public void Init()
    {
        _building = GetComponent<Building>();

        _building.DestroyedHandler += OnDestroyed;
    }

    void OnDestroyed()
    {
        Manager.Item.GenerateItem(data.GetRandom(), transform.position);
        _building.DestroyedHandler -= OnDestroyed;
    }
}
