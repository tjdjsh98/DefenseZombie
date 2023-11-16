using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class Smithy : InteractableObject, IBuildingOption, IEnableInsertItem
{
    public int _reinforceItemId;
    public int ReinforceItemId => _reinforceItemId;
    public Action ReinforeceItemSet;

    [System.Serializable]
    public class RequireItems
    {
        public List<RequireItem> requireItems;
    }
    [System.Serializable]
    public class RequireItem
    {
        public ItemName itemName;
        public int requireCount;
        public int currentCount;
        public List<int> itemIdList = new List<int>();
        public void AddCount(int itemId)
        {
            currentCount++;
            itemIdList.Add(itemId);
        }
    }

    [SerializeField] List<RequireItems> _requireItemList;
    public List<RequireItems> RequireItemList=>_requireItemList;

    public Action<bool> ItemChangedHandler { get; set; }

    public void Init()
    {

    }


    public override bool Interact(Character character)
    {
        CharacterEquipment characterEquipment = character.GetComponent<CharacterEquipment>();
        if(characterEquipment== null) return false;

        AddEnforeceItem(characterEquipment.WeaponId);

        return true;

    }

    public override bool ExitInteract(Character character)
    {
        return true;
    }
    public void AddEnforeceItem(int itemId)
    {
        Item item = Manager.Item.GetItem(itemId);
        if (item == null) return;

        if (item.ItemData.ItemType == Define.ItemType.Equipment)
        {

            // 캐릭터에게 아이템을 벗긴다.
            CharacterEquipment characterEquipment = Manager.Character.GetCharacter(item.GrapedCharacterId).GetComponent<CharacterEquipment>();
            if (characterEquipment != null)
            {
                // 아이템이 무기일 경우
                if (item.ItemData.EquipmentData == null)
                {
                    characterEquipment.TakeOffWeapon();
                }
                // 아이템이 방어구 일 경우
                else
                {
                    characterEquipment.TakeOffOther(item.ItemData.EquipmentData.CharacterParts);
                }
                _reinforceItemId = itemId;
                item.Hide();
            }
        }
        ReinforeceItemSet?.Invoke();
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

    public bool InsertItem(Item item)
    {
        bool isSuccess = false;
        ItemEquipment itemEquipment = Manager.Item.GetItem(_reinforceItemId).GetComponent<ItemEquipment>();

        if (itemEquipment == null) return isSuccess;

        for (int i = 0; i < _requireItemList[itemEquipment.Rank].requireItems.Count; i++)
        {
            if (_requireItemList[itemEquipment.Rank].requireItems[i].itemName== item.ItemData.ItemName)
            {
                if (_requireItemList[itemEquipment.Rank].requireItems[i].requireCount > _requireItemList[itemEquipment.Rank].requireItems[i].currentCount)
                {
                    _requireItemList[itemEquipment.Rank].requireItems[i].AddCount(item.ItemId);
                    item.Hide();
                    bool isFinish = CheckIsFinish();
                    ItemChangedHandler?.Invoke(isFinish);
                    isSuccess = true;
                    break;
                }
            }
        }
        //if (isSuccess && Client.Instance.IsMain)
        //{
        //    Client.Instance.SendBuildingInfo(_building);
        //}

        return isSuccess;
    }

    public bool CheckIsFinish()
    {
        ItemEquipment itemEquipment = Manager.Item.GetItem(_reinforceItemId).GetComponent<ItemEquipment>();

        if (itemEquipment == null) return false;

        bool isSuccess = true;

        for (int i = 0; i < _requireItemList[itemEquipment.Rank].requireItems.Count; i++)
        {
            if (_requireItemList[itemEquipment.Rank].requireItems[i].requireCount > _requireItemList[itemEquipment.Rank].requireItems[i].currentCount)
            {
                isSuccess = false;
                break;
            }
        }

        if (isSuccess)
        {
            foreach (var requireItem in _requireItemList[itemEquipment.Rank].requireItems)
            {
                foreach (var id in requireItem.itemIdList)
                {
                    Manager.Item.RemoveItem(id);
                }

                requireItem.currentCount = 0;
                requireItem.itemIdList.Clear();
            }

            Item item = null;
            int random = Random.Range(0, 3);

            if(random == 0) itemEquipment.AddSpeed(Random.Range(0.2f, 0.6f));
            if(random == 1) itemEquipment.AddDefense(Random.Range(0, 2));
            if(random == 2) itemEquipment.AddHp(Random.Range(3,5));

            item = Manager.Item.GetItem(_reinforceItemId);
            item.Show();
        }

        return isSuccess;
    }
}
