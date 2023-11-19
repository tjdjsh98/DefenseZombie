using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class Smithy : InteractableObject, IBuildingOption, IEnableInsertItem
{
    Building _building;

    public int _reinforceItemId;
    public int _requestReinforceItemId;
    public int _rank;
  

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

    UI_Smithy _ui;

    bool _initDone = false;
    Character _openCharacter = null;

    public void Init()
    {
        _building = GetComponent<Building>();
        _initDone = true;
    }

    public void Update()
    {
        if (!_initDone) return;


        if (_openCharacter != null)
        {
            if ((transform.position - _openCharacter.transform.position).magnitude > 2)
            {
                _openCharacter.GetComponent<PlayerController>().ExitInteract();
            }
        }
    }

    public override bool Interact(Character character)
    {
        if (!_building.IsConstructDone) return false;

        if(_ui == null)
            _ui = Manager.UI.GetUI(UIName.Smithy) as UI_Smithy;

        _ui.Open(this, character as CustomCharacter);
        character.IsEnableAttack = false;
        _openCharacter = character;
        return true;

    }

    public override bool ExitInteract(Character character)
    {
        _ui.Close();
        _openCharacter.IsEnableAttack = true;
        _openCharacter = null;

        return true;
    }
    public void SetEnforeceItem(int itemId)
    {
        if (Client.Instance.IsSingle || Client.Instance.IsMain)
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
                    _rank = item.GetComponent<ItemEquipment>().Rank;
                    item.Hide();

                    foreach (var i in _requireItemList[_rank].requireItems)
                    {
                        i.itemIdList.Clear();
                        i.currentCount = 0;
                    }
                }

            }
            ReinforeceItemSet?.Invoke();
            Client.Instance.SendBuildingInfo(_building);
        }
        else
        {
            _requestReinforceItemId = itemId;
            Client.Instance.SendBuildingControlInfo(_building);
            _requestReinforceItemId = 0;
        }
    }
    public bool InsertItem(Item item)
    {
        bool isSuccess = false;
        ItemEquipment itemEquipment = Manager.Item.GetItem(_reinforceItemId).GetComponent<ItemEquipment>();

        if (itemEquipment == null) return isSuccess;

        for (int i = 0; i < _requireItemList[_rank].requireItems.Count; i++)
        {
            if (_requireItemList[_rank].requireItems[i].itemName == item.ItemData.ItemName)
            {
                if (_requireItemList[_rank].requireItems[i].requireCount > _requireItemList[_rank].requireItems[i].currentCount)
                {
                    _requireItemList[_rank].requireItems[i].AddCount(item.ItemId);
                    item.Hide();
                    bool isFinish = CheckIsFinish();
                    ItemChangedHandler?.Invoke(isFinish);
                    isSuccess = true;
                    break;
                }
            }
        }
        if (isSuccess && Client.Instance.IsMain)
        {
            Client.Instance.SendBuildingInfo(_building);
        }

        return isSuccess;
    }

    public bool CheckIsFinish()
    {
        ItemEquipment itemEquipment = Manager.Item.GetItem(_reinforceItemId).GetComponent<ItemEquipment>();

        if (itemEquipment == null) return false;

        bool isSuccess = true;

        for (int i = 0; i < _requireItemList[_rank].requireItems.Count; i++)
        {
            if (_requireItemList[_rank].requireItems[i].requireCount > _requireItemList[_rank].requireItems[i].currentCount)
            {
                isSuccess = false;
                break;
            }
        }

        if (isSuccess)
        {
            foreach (var requireItem in _requireItemList[_rank].requireItems)
            {
                foreach (var id in requireItem.itemIdList)
                {
                    if(Client.Instance.IsSingle || Client.Instance.IsMain) 
                    { 
                        Manager.Item.RemoveItem(id);
                    }
                }

                requireItem.itemIdList.Clear();
            }

            if (Client.Instance.IsSingle || Client.Instance.IsMain)
            {
                Item item = null;
                item = Manager.Item.GetItem(_reinforceItemId);
                item.Show();

                // 무기라면 공격력을 올립니다.
                if (item.ItemData.ItemType == ItemType.Equipment && item.ItemData.EquipmentData == null)
                {
                    itemEquipment.AddAttack(Random.Range(1, 3));
                }
                // 방어구라면 랜덤한 것을 올립니다.
                else
                {
                    int random = Random.Range(0, 3);

                    if (random == 0) itemEquipment.AddSpeed(((int)(Random.Range(0.2f, 0.6f) * 100)) / 100f);
                    if (random == 1) itemEquipment.AddDefense(Random.Range(0, 2));
                    if (random == 2) itemEquipment.AddHp(Random.Range(3, 5));
                }
            }
        }

        return isSuccess;
    }
    public void SerializeData()
    {
        Util.WriteSerializedData(_reinforceItemId);
        Util.WriteSerializedData(_rank);
        if (_reinforceItemId > 0)
        {
            Util.WriteSerializedData(_requireItemList[_rank].requireItems.Count);
            for (int i = 0; i < _requireItemList[_rank].requireItems.Count; i++)
            {
                int name = (int)_requireItemList[_rank].requireItems[i].itemName;
                Util.WriteSerializedData(name);
                int requireCount = _requireItemList[_rank].requireItems[i].requireCount;
                Util.WriteSerializedData(requireCount);
                int currentCount = _requireItemList[_rank].requireItems[i].currentCount;
                Util.WriteSerializedData(currentCount);

                Util.WriteSerializedData(_requireItemList[_rank].requireItems[i].itemIdList.Count);
                for (int j = 0; j < _requireItemList[_rank].requireItems[i].itemIdList.Count; j++)
                {
                    int id = _requireItemList[_rank].requireItems[i].itemIdList[j];
                    Util.WriteSerializedData(id);
                }
            }
        }
        else
            Util.WriteSerializedData(0);
    }
    public void DeserializeData()
    {
        int reinforceItemId = Util.ReadSerializedDataToInt();
        int rank = Util.ReadSerializedDataToInt();
        

        if (_reinforceItemId != reinforceItemId  || rank != _rank)
        {
            _reinforceItemId = reinforceItemId;
            _rank = rank;
            ReinforeceItemSet?.Invoke();
        }
        _reinforceItemId = reinforceItemId;
        _rank = rank;

        int requireItemCount = Util.ReadSerializedDataToInt();


        for (int i = 0; i < requireItemCount; i++)
        {
            int name = Util.ReadSerializedDataToInt();
            int requireCount = Util.ReadSerializedDataToInt();
            int currentCount = Util.ReadSerializedDataToInt();

            _requireItemList[_rank].requireItems[i].itemName = (ItemName)name;
            _requireItemList[_rank].requireItems[i].requireCount = requireCount;
            _requireItemList[_rank].requireItems[i].currentCount = currentCount;
           
            int itemIdCount = Util.ReadSerializedDataToInt();

            _requireItemList[_rank].requireItems[i].itemIdList.Clear();

            for (int j = 0; j < itemIdCount; j++)
            {
                int id = Util.ReadSerializedDataToInt();
                if (_requireItemList[_rank].requireItems[i].itemIdList.Count >= j)
                    _requireItemList[_rank].requireItems[i].itemIdList.Add(id);
                else
                    _requireItemList[_rank].requireItems[i].itemIdList[j] = id;
            }
        }
        ItemChangedHandler?.Invoke(CheckIsFinish());
    }

    public void SerializeControlData()
    {
        Util.WriteSerializedData(_requestReinforceItemId);
    }

    public void DeserializeControlData()
    {
        int requestIndex = Util.ReadSerializedDataToInt();
        if (requestIndex > 0 && Client.Instance.IsMain)
        {
            SetEnforeceItem(requestIndex);
        }
    }




    
}
