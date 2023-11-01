using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

[RequireComponent(typeof(Building))]
public class Smithy : InteractableObject, IBuildingOption, IEnableInsertItem
{
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

    public bool InsertItem(ItemName itemName)
    {
        if (_mainBlueprint == null) return false;

        for (int i = 0; i < _mainBlueprint.BlueprintItemList.Count; i++)
        {
            if (_mainBlueprint.BlueprintItemList[i].name == itemName)
            {
                _mainBlueprint.BlueprintItemList[i].AddCount();
                ItemChangedHandler?.Invoke(CheckIsFinish());
                return true;
            }
        }

        return false;
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
            Manager.Item.GenerateItem(MainBlueprint.ResultItemName, transform.position);
            _mainBlueprint = null;
        }

        return isSuccess;
    }
}
