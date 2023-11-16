using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ItemManager : MonoBehaviour
{
    static int itemNumber = 0;

    Dictionary<int, Item> _itemDictionary= new Dictionary<int, Item>();

    Dictionary<int, Action<Item>> _requestGenerateAction = new Dictionary<int, Action<Item>>();
    Dictionary<int, Action> _requsetRemoveAction = new Dictionary<int, Action>();

    public void Init()
    {
        Item item = null;
        GenerateItem(ItemName.Wood, Vector3.zero,ref item);
        GenerateItem(ItemName.Wood, Vector3.zero, ref item);
        GenerateItem(ItemName.Wood, Vector3.zero, ref item);

        if(!Client.Instance.IsSingle && Client.Instance.IsMain)
        {
            StartCoroutine(SendPacketCor());
        }
    }


    // 싱글, 멀티 혼용으로 아이템 생성

    public int GenerateItem(ItemName itemName, Vector3 position, ref Item item, Action<Item> addAction = null)
    {
        int requestNumber = -1;

        if (Client.Instance.IsSingle)
        {
            item = GenerateItem(itemName, position);
        }
        else
        {
            requestNumber = RequestGenerateItem(itemName, position,addAction);
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

        item.RandomBounding(10);

        _itemDictionary.Add(item.ItemId, item);

        return item;
    }

    // 멀티 일 때 아이템 생성 서버에 요청
    private int RequestGenerateItem(ItemName itemName, Vector3 position, Action<Item> addAction = null)
    {
        int requestNumber = UnityEngine.Random.Range(300, 700);

        if (addAction != null)
        {
            _requestGenerateAction.Add(requestNumber, addAction);
        }
            

        Client.Instance.SendRequestGeneratingItem(itemName,position, requestNumber);

        return requestNumber;
    }
    // 싱글, 멀티 혼용으로 사용하는 아이템 삭제

    public int RemoveItem(int itemId)
    {
        int requestNumber = -1;

        if(Client.Instance.IsSingle)
        {
            SingleRemoveItem(itemId);
        }
        else
        {
            requestNumber = RequestRemoveItem(itemId);
        }

        return requestNumber;
    }

    // 멀티 일 때 아이템 삭제 서버에 요청
    private int RequestRemoveItem(int itemId,Action removeAction = null)
    {
        int requestNumber = UnityEngine.Random.Range(500, 1000);

        if (removeAction != null)
        {
            _requsetRemoveAction.Add(requestNumber, removeAction);
        }

        Client.Instance.SendRequestRemoveItem(itemId, requestNumber);

        return requestNumber;
    }


    // 싱글일 때 아이템 삭제 
    private bool SingleRemoveItem(int itemId)
    {
        Item item = null;

        if(_itemDictionary.TryGetValue(itemId, out item))
        {
            GameObject.Destroy(item.gameObject);

            _itemDictionary.Remove(itemId);

            return true;
        }

        return false;
    }


    // 받은 패킷으로 아이템 생성
    public Item GenerateItemByPacket(S_BroadcastGenerateItem packet)
    {
        ItemData itemData = Manager.Data.GetItemData((ItemName)packet.itemName);

        if (itemData == null) return null;

        if(_itemDictionary.TryGetValue(packet.itemId,out var tempItem))
        {
            _itemDictionary.Remove(packet.itemId);
            Destroy(tempItem.gameObject);
        }

        GameObject origin = null;

        if (itemData.Origin == null)
            origin = Manager.Data.GetEtc("Item");
        else
            origin = itemData.Origin;

        Item item = Instantiate(origin).GetComponent<Item>();

        item.Init(itemData,packet.itemId);

        Vector3 position = new Vector3(packet.posX, packet.posY);
        item.transform.position = position;

        item.RandomBounding(10);

        _itemDictionary.Add(item.ItemId, item);


        if(_requestGenerateAction.ContainsKey(packet.requestNumber))
        {
            _requestGenerateAction[packet.requestNumber]?.Invoke(item);
            _requestGenerateAction.Remove(packet.requestNumber);
        }

        return item;
    }
    public void GenerateItemByPacket(S_EnterSyncInfos packet)
    {
        foreach(var info in packet.itemInfos)
        {
            if (_itemDictionary.ContainsKey(info.itemId))
            {
                Destroy(_itemDictionary[info.itemId].gameObject);
                _itemDictionary.Remove(info.itemId);
            }

            ItemData itemData = Manager.Data.GetItemData((ItemName)info.itemName);

            if(itemData == null) continue;

            GameObject origin = null;

            if (itemData.Origin == null)
                origin = Manager.Data.GetEtc("Item");
            else
                origin = itemData.Origin;
            Item item = Instantiate(origin).GetComponent<Item>();

            item.Init(itemData, info.itemId);

            Vector3 position = new Vector3(info.posX, info.posY);
            item.transform.position = position;

            item.RandomBounding(10);

            _itemDictionary.Add(item.ItemId, item);

        }
    }

    // 받은 패킷으로 아이템 삭제

    public void RemoveItemByPacket(S_BroadcastRemoveItem packet)
    {
        if (!packet.isSuccess) return;

        int itemId = packet.itemId;

        Item item = null;

        if (_itemDictionary.TryGetValue(itemId, out item))
        {
            GameObject.Destroy(item.gameObject);

            _itemDictionary.Remove(itemId);
        }

        if (_requsetRemoveAction.ContainsKey(packet.requestNumber))
        {
            _requsetRemoveAction[packet.requestNumber]?.Invoke();
            _requsetRemoveAction.Remove(packet.requestNumber);
        }
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

    IEnumerator SendPacketCor()
    {
        while (true)
        {
            foreach (var item in _itemDictionary.Values)
            {
                Client.Instance.SendItemInfo(item);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
