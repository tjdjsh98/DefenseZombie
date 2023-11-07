using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

[RequireComponent(typeof(Building))]
public class Smithy : InteractableObject, IBuildingOption, IEnableInsertItem
{
    Building _building;
    BlueprintDisplayer _itemDisplayer;

    [SerializeField] List<ItemBlueprintData> _itemBlueprintDataList;
    public List<ItemBlueprintData> ItemBlueprintDataList => _itemBlueprintDataList;

    Character _openCharacter;
    UI_Smithy _ui;

    bool _initDone;

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
        if (_openCharacter != null || _mainBlueprint != null) return false;

        _openCharacter = character;
        character.IsEnableAttack = false;

        if(_ui == null)_ui = Manager.UI.GetUI(Define.UIName.Smithy) as UI_Smithy;

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

    public void SetMainBlueprint(ItemBlueprintData data)
    {
        _mainBlueprint = new ItemBlueprintData();
        _mainBlueprint.BlueprintItemList = data.BlueprintItemList.ConvertAll(o =>  new BlueprintItem(o));
        _mainBlueprint.ResultItemName = data.ResultItemName;

        MainBlueprintSetHandler?.Invoke();
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
        }

        return isSuccess;
    }

    public void DataSerialize()
    {
    }

    public void DataDeserialize()
    {
    }
}
