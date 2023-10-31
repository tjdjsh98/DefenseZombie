using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ItemManager : MonoBehaviour
{
    static int itemNumber = 0;

    Dictionary<int, Item> _itemDictionary= new Dictionary<int, Item>();


    public void Init()
    {
        GenerateItem(ItemName.Wood, Vector3.zero);
        GenerateItem(ItemName.Wood, Vector3.zero);
        GenerateItem(ItemName.Wood, Vector3.zero);
    }

    public Item GenerateItem(ItemName itemName, Vector3 position)
    {
        ItemData itemData = Manager.Data.GetItemData(itemName);

        if (itemData == null) return null;

        GameObject origin = null;

        if (itemData.Origin == null)
            origin = Manager.Data.GetEtc("Item");
        else
            origin = itemData.Origin;

        Item item = Instantiate(origin).GetComponent<Item>();

        item.Init(itemData, ++itemNumber);

        position.z = 0;
        item.transform.position = position;

        item.UpBound(10);

        _itemDictionary.Add(item.ItemId, item);

        return item;
    }

    public Item GetItem(int itemNumber)
    {
        Item item = null;
        _itemDictionary.TryGetValue(itemNumber, out item);
        
        return item;
    }

    public bool DestroyItem(int itemNumber)
    {
        Item item = null;

        if(_itemDictionary.TryGetValue(itemNumber, out item))
        {
            Destroy(item.gameObject);
            _itemDictionary.Remove(itemNumber);
        }
        return false;
    }
}
