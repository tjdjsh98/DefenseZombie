using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Smithy : UIBase
{
    Character _openCharacter;
    Smithy _smithy;

    [SerializeField] GameObject _leftButton;
    [SerializeField] GameObject _rightButton;

    [SerializeField]List<Image> _slotImageList = new List<Image>();
    [SerializeField]List<Image> _slotBackList = new List<Image>();
    [SerializeField]List<Image> _stuffItemImageList = new List<Image>();
    List<TextMeshProUGUI> _stuffItemCountList = new List<TextMeshProUGUI>();

    int _page = 0;

    int _selectIndex = 0;
    float _imageSize = 80;

    public override void Init()
    {
        _isDone = true;
        foreach(var image in _stuffItemImageList)
        {
            _stuffItemCountList.Add(image.GetComponentInChildren<TextMeshProUGUI>());
        }

        gameObject.SetActive(false);
    }

    public void Open(Character character, Smithy smithy)
    {
        _openCharacter = character;
        _smithy = smithy;

        _page = 0;
        _leftButton.gameObject.SetActive(false);

        if ((_page + 1) * _slotImageList.Count >= _smithy.ItemBlueprintDataList.Count)
        {
            _rightButton.gameObject.SetActive(false);
        }
        else
        {
            _rightButton.gameObject.SetActive(true);
        }

        Refresh();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);

        _smithy = null;
        _openCharacter = null;

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void RightButton()
    {
        _page++;
        if ((_page + 1) * _slotImageList.Count > _smithy.ItemBlueprintDataList.Count)
        {
            _rightButton.gameObject.SetActive(false);
        }else
        {
            _rightButton.gameObject.SetActive(true);
        }

        if (_page != 0)
        {

            _leftButton.gameObject.SetActive(true);
        }
        else
            _leftButton.gameObject.SetActive(false);
        Refresh();
    }

    public void LeftButton()
    {
        _page--;
        if (_page <= 0)
        {
            _leftButton.gameObject.SetActive(false);
        }
        else
        {
            _leftButton.gameObject.SetActive(true);
        }

        if ((_page + 1) * _slotImageList.Count > _smithy.ItemBlueprintDataList.Count)
        {
            _rightButton.gameObject.SetActive(false);
        }
        else
        {
            _rightButton.gameObject.SetActive(true);
        }
        Refresh();
    }


    void Refresh()
    {
        int count = 0;
        if (_slotImageList.Count < _smithy.ItemBlueprintDataList.Count - _page* _slotImageList.Count)
        {
            count = _slotImageList.Count;
        }
        else
        {
            count = _smithy.ItemBlueprintDataList.Count - _page * _slotImageList.Count;
        }
        int i = 0;
        for (i = 0; i < count; i++)
        {
            ItemName itemName = _smithy.ItemBlueprintDataList[_page*_slotImageList.Count + i].ResultItemName;
            ItemData result = Manager.Data.GetItemData(itemName);
            _slotImageList[i].sprite = result.ItemSprite;
            _slotImageList[i].rectTransform.sizeDelta  = Util.CalcFitSize(_imageSize, result.ItemSprite);
            _slotImageList[i].transform.parent.localPosition = new Vector3(-180 * (count - 1) / 2f + 180 * i, _slotImageList[i].transform.parent.localPosition.y);
            _slotBackList[i].gameObject.SetActive(true);
        }
        for(;i< _slotImageList.Count;i++)
        {
            _slotBackList[i].gameObject.SetActive(false);
        }


        PushSelectButton(0);
    }


    public void PushSelectButton(int index)
    {
        if(_slotBackList.Count > _selectIndex && _selectIndex >= 0) 
            _slotBackList[_selectIndex].color = Color.white;
        _selectIndex = index;
        _slotBackList[_selectIndex].color = Color.green;

        int i = 0;
        for(; i < _smithy.ItemBlueprintDataList[_page * _slotBackList.Count + _selectIndex].BlueprintItemList.Count; i++)
        {
            ItemName itemName = _smithy.ItemBlueprintDataList[_page* _slotBackList.Count + _selectIndex].BlueprintItemList[i].name;
            _stuffItemImageList[i].sprite = Manager.Data.GetItemData(itemName).ItemSprite;
            _stuffItemImageList[i].rectTransform.sizeDelta = Util.CalcFitSize(_imageSize, _stuffItemImageList[i].sprite);
            _stuffItemCountList[i].text = _smithy.ItemBlueprintDataList[_page * _slotBackList.Count + _selectIndex].BlueprintItemList[i].requireCount.ToString();

            _stuffItemImageList[i].transform.localPosition = new Vector3
                (-(_smithy.ItemBlueprintDataList[_page * _slotBackList.Count + _selectIndex].BlueprintItemList.Count-1)/2f*120f + i*120f,
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
        _smithy.SetMainBlueprint(_page * _slotBackList.Count + _selectIndex);

        Close();
    }
}
