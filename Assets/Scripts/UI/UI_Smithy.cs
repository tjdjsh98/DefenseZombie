using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Smithy : UIBase
{
    Character _openCharacter;
    Smithy _smithy;

    [SerializeField]List<Image> _slotImageList = new List<Image>();
    [SerializeField]List<Image> _stuffItemImageList = new List<Image>();

    int _selectIndex = 0;
    public override void Init()
    {
        _isDone = true;
        gameObject.SetActive(false);
    }

    public void Open(Character character, Smithy smithy)
    {
        _openCharacter = character;
        _smithy = smithy;

        Refresh();

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);

        _smithy = null;
        _openCharacter = null;

    }


    void Refresh()
    {
        for (int i = 0; i < _slotImageList.Count; i++)
        {
            ItemName itemName = _smithy.ItemBlueprintDataList[i].ResultItemName;
            ItemData result = Manager.Data.GetItemData(itemName);
            _slotImageList[i].sprite = result.ItemSprite;
        }

        PushSelectButton(0);
    }


    public void PushSelectButton(int index)
    {
        _selectIndex = index;

        int i = 0;
        for(; i < _smithy.ItemBlueprintDataList[_selectIndex].BlueprintItemList.Count; i++)
        {
            ItemName itemName = _smithy.ItemBlueprintDataList[_selectIndex].BlueprintItemList[i].name;
            _stuffItemImageList[i].sprite = Manager.Data.GetItemData(itemName).ItemSprite;

            _stuffItemImageList[i].transform.localPosition = new Vector3
                (-(_smithy.ItemBlueprintDataList[_selectIndex].BlueprintItemList.Count-1)/2f*120f + i*120f,
                _stuffItemImageList[i].transform.localPosition.y,0);

            _stuffItemImageList[i].gameObject.SetActive(true);
        }
        for (; i < _stuffItemImageList.Count; i++)
        {
            _stuffItemImageList[i].gameObject.SetActive(false);
        }
    }

    public void PushCreateButton()
    {
        _smithy.SetMainBlueprint(_selectIndex);

        Close();
    }
}
