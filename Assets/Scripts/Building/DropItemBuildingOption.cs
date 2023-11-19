using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DropItemBuildingOption : MonoBehaviour, IBuildingOption
{
    Building _building;

    [SerializeField] List<ItemName> data;

    public void DeserializeData()
    {
    }

    public void SerializeData()
    {
    }

    public void Init()
    {
        _building = GetComponent<Building>();

        _building.DestroyedHandler += OnDestroyed;
    }

    void OnDestroyed()
    {
        if (Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            Item item = null;
            foreach (var itemName in data)
            {
                Manager.Item.GenerateItem(itemName, transform.position, ref item);
            }
            _building.DestroyedHandler -= OnDestroyed;
        }
    }

    public void SerializeControlData()
    {
    }

    public void DeserializeControlData()
    {
    }
}
