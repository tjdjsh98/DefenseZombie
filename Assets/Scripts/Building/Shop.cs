using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

[RequireComponent(typeof(Building))]
public class Shop : InteractableObject, IBuildingOption, IEnableInsertItem
{
    Building _building;
    BlueprintDisplayer _itemDisplayer;

    [SerializeField] List<ItemBlueprintData> _itemBlueprintDataList;
    public List<ItemBlueprintData> ItemBlueprintDataList => _itemBlueprintDataList;

    Character _openCharacter;
    UI_Shop _ui;

    bool _initDone;

    int _requsetBlueprintIndex = -1;
    int _mainBlueprintIndex = -1;

    ItemBlueprintData _mainBlueprint;
    public ItemBlueprintData MainBlueprint => _mainBlueprint;

    public Action<bool> ItemChangedHandler { get; set; }

    public Action MainBlueprintSetHandler { set; get; }

    public void Init()
    {
        _initDone = true;
        _building = GetComponent<Building>();
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
        if (_openCharacter != null || _mainBlueprint != null) return false;

        _openCharacter = character;
        character.IsEnableAttack = false;

        if(_ui == null)_ui = Manager.UI.GetUI(Define.UIName.Shop) as UI_Shop;

        _ui.Open(character,this);

        return true;
    }

    public override bool ExitInteract(Character character)
    {
        _ui.Close();
        character.IsEnableAttack = true;
        _openCharacter = null;

        return true;
    }

    public void SetMainBlueprint(int index)
    {
        if (Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            _mainBlueprintIndex = index;
            ItemBlueprintData data = ItemBlueprintDataList[index];
            _mainBlueprint = new ItemBlueprintData();
            _mainBlueprint.BlueprintItemList = data.BlueprintItemList.ConvertAll(o => new BlueprintItem(o));
            _mainBlueprint.ResultItemName = data.ResultItemName;
             MainBlueprintSetHandler?.Invoke();

            Client.Instance.SendBuildingInfo(_building);
        }
        else
        {
            _requsetBlueprintIndex = index;
            Client.Instance.SendBuildingControlInfo(_building);
            _requsetBlueprintIndex = -1;
        }

    }

    public bool InsertItem(Item item)
    {
        bool isSuccess = false;
        if (_mainBlueprint == null) return isSuccess;

        for (int i = 0; i < _mainBlueprint.BlueprintItemList.Count; i++)
        {
            if (_mainBlueprint.BlueprintItemList[i].name == item.ItemData.ItemName)
            {
                if (_mainBlueprint.BlueprintItemList[i].requireCount > _mainBlueprint.BlueprintItemList[i].currentCount)
                {
                    _mainBlueprint.BlueprintItemList[i].AddCount(item.ItemId);
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
        if (_mainBlueprint == null) return false;

        bool isSuccess = true;

        for (int i = 0; i < _mainBlueprint.BlueprintItemList.Count; i++)
        {
            if (_mainBlueprint.BlueprintItemList[i].requireCount > _mainBlueprint.BlueprintItemList[i].currentCount)
            {
                isSuccess = false;
                break;
            }
        }

        if (isSuccess)
        {
            foreach (var blueprint in _mainBlueprint.BlueprintItemList)
            {
                foreach (var id in blueprint.itemIdList)
                {
                    Manager.Item.RemoveItem(id);
                }

                blueprint.itemIdList.Clear();
            }

            Item item = null;
            Manager.Item.GenerateItem(MainBlueprint.ResultItemName, transform.position,ref item);
            _mainBlueprint = null;
            _mainBlueprintIndex = -1;
        }

        return isSuccess;
    }

    public void SerializeData()
    {
        Util.WriteSerializedData(_mainBlueprintIndex);
        if (_mainBlueprint != null)
        {
            Util.WriteSerializedData(_mainBlueprint.BlueprintItemList.Count);
            for (int i = 0; i < _mainBlueprint.BlueprintItemList.Count; i++)
            {
                int name = (int)_mainBlueprint.BlueprintItemList[i].name;
                Util.WriteSerializedData(name);
                int requireCount = _mainBlueprint.BlueprintItemList[i].requireCount;
                Util.WriteSerializedData(requireCount);
                int currentCount = _mainBlueprint.BlueprintItemList[i].currentCount;
                Util.WriteSerializedData(currentCount);

                Util.WriteSerializedData(_mainBlueprint.BlueprintItemList[i].itemIdList.Count);
                for (int j = 0; j < _mainBlueprint.BlueprintItemList[i].itemIdList.Count; j++)
                {
                    int id = _mainBlueprint.BlueprintItemList[i].itemIdList[j];
                    Util.WriteSerializedData(id);
                }
            }
        }
        else
            Util.WriteSerializedData(0);
    }

    public void DeserializeData()
    {
        int mainBlueprintIndex = Util.ReadSerializedDataToInt();

        if (_mainBlueprintIndex == -1 && mainBlueprintIndex != -1)
        {
            _mainBlueprintIndex = mainBlueprintIndex;
            ItemBlueprintData data = ItemBlueprintDataList[mainBlueprintIndex];
            _mainBlueprint = ScriptableObject.CreateInstance<ItemBlueprintData>();
            _mainBlueprint.BlueprintItemList = data.BlueprintItemList.ConvertAll(o => new BlueprintItem(o));
            _mainBlueprint.ResultItemName = data.ResultItemName;
            MainBlueprintSetHandler?.Invoke();
        }
        else if(_mainBlueprintIndex != -1 && mainBlueprintIndex == -1) 
        {
            _mainBlueprint = null;
            _mainBlueprintIndex = -1;
            ItemChangedHandler?.Invoke(true);
        }

        int blueprintItemCount = Util.ReadSerializedDataToInt();
        for (int i = 0; i < blueprintItemCount; i++)
        {
            int name = Util.ReadSerializedDataToInt();
            int requireCount = Util.ReadSerializedDataToInt();
            int currentCount = Util.ReadSerializedDataToInt();

            if (_mainBlueprint.BlueprintItemList.Count <= i)
            {
                _mainBlueprint.BlueprintItemList.Add(new BlueprintItem((ItemName)name, requireCount, currentCount));
            }
            else
            {
                _mainBlueprint.BlueprintItemList[i].name = (ItemName)name;
                _mainBlueprint.BlueprintItemList[i].requireCount = requireCount;
                _mainBlueprint.BlueprintItemList[i].currentCount = currentCount;
            }
            int itemIdCount = Util.ReadSerializedDataToInt();

            _mainBlueprint.BlueprintItemList[i].itemIdList.Clear();

            for (int j = 0; j < itemIdCount; j++)
            {
                int id = Util.ReadSerializedDataToInt();
                if (_mainBlueprint.BlueprintItemList[i].itemIdList.Count >= j)
                    _mainBlueprint.BlueprintItemList[i].itemIdList.Add(id);
                else
                    _mainBlueprint.BlueprintItemList[i].itemIdList[j] = id;
            }
        }
        ItemChangedHandler?.Invoke(CheckIsFinish());
    }

    public void SerializeControlData()
    {
        Util.WriteSerializedData(_requsetBlueprintIndex);
    }

    public void DeserializeControlData()
    {
        int requestIndex = Util.ReadSerializedDataToInt();
        if (requestIndex <= 0 && Client.Instance.IsMain)
        {
            SetMainBlueprint(requestIndex);
        }

    }
}
