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
        GenerateItem(ItemName.Stone, Vector3.one * 3);
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

        _itemDictionary.Add(item.ItemNumber, item);

        return item;
    }

    public Item GetItem(int itemNumber)
    {
        Item item = null;
        _itemDictionary.TryGetValue(itemNumber, out item);
        
        return item;
    }
}
