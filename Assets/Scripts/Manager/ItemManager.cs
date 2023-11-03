using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class ItemManager : MonoBehaviour
{
    static int itemNumber = 0;

    Dictionary<int, Item> _itemDictionary= new Dictionary<int, Item>();


    public void Init()
    {
        Item item = null;
        GenerateItem(ItemName.Wood, Vector3.zero,ref item);
        GenerateItem(ItemName.Wood, Vector3.zero, ref item);
        GenerateItem(ItemName.Wood, Vector3.zero, ref item);
    }


    // 싱글, 멀티 혼용으로 아이템 생성

    public int GenerateItem(ItemName itemName, Vector3 position, ref Item item)
    {
        int requestNumber = -1;

        if (Client.Instance.IsSingle)
        {
            item = GenerateItem(itemName, position);
        }
        else
        {
            requestNumber = RequestGenerateItem(itemName, position);
        }

        return requestNumber;
    }

    // 싱글 일 때 아이템 생성
    private Item GenerateItem(ItemName itemName, Vector3 position)
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

    // 멀티 일 때 아이템 생성 서버에 요청
    private int RequestGenerateItem(ItemName itemName, Vector3 position)
    {
        int requestNumber = UnityEngine.Random.Range(300, 700);

        Client.Instance.SendRequestGeneratingItem(itemName,position, requestNumber);

        return requestNumber;
    }
    // 싱글, 멀티 혼용으로 사용하는 아이템 삭제



    // 멀티 일 때 아이템 삭제 서버에 요청

    // 싱글일 때 아이템 삭제 


    // 받은 패킷으로 아이템 생성
    public Item GenerateItemByPacket(S_BroadcastGenerateItem packet)
    {
        ItemData itemData = Manager.Data.GetItemData((ItemName)packet.itemName);

        if (itemData == null) return null;

        GameObject origin = null;

        if (itemData.Origin == null)
            origin = Manager.Data.GetEtc("Item");
        else
            origin = itemData.Origin;

        Item item = Instantiate(origin).GetComponent<Item>();

        item.Init(itemData,packet.itemId);

        Vector3 position = new Vector3(packet.posX, packet.posY);
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
