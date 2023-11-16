using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Build : UIBase
{
    [System.Serializable]
    class BuildingNameSprite
    {
        public BuildingName buildingName;
        public Sprite buildingSprite;
        public Image image;
    }
    [SerializeField] GameObject _rightButton;
    [SerializeField] GameObject _leftButton;

    [SerializeField]List<BuildingName> _buildingList = new List<BuildingName>();
    [SerializeField]List<BuildingNameSprite> _buildingInfo = new List<BuildingNameSprite>();

    UI_Description _uiDescription = null;
    UI_Description UIDescription { get
        {
            if (_uiDescription == null)
            {
                _uiDescription = Manager.UI.GetUI(UIName.Description) as UI_Description;
            } return _uiDescription; } }

    int _page;

    bool _isOpen=false;
    public override void Init()
    {
        Manager.Input.UIMouseDownHandler += OnUIMouseDown;
        Manager.Input.UIMouse1DownHandler += OnMouse1Down;


        _leftButton.gameObject.SetActive(false);
        if ((_page + 1) * _buildingInfo.Count >= _buildingList.Count)
        {
            _rightButton.gameObject.SetActive(false);
        }

        Refresh();
        _isDone = true;
    }

    public void PageRight()
    {
        _page++;
        if ((_page + 1) * _buildingInfo.Count >=_buildingList.Count)
        {
            _rightButton.gameObject.SetActive(false);
        }
        else
        {
            _rightButton.gameObject.SetActive(true);
        }

        if (_page == 0)
        {
            _leftButton.gameObject.SetActive(false);
        }
        else
        {
            _leftButton.gameObject.SetActive(true);
        }
        Refresh();
    }

    public void PageLeft()
    {
        _page--;
        if (_page == 0)
        {
            _leftButton.gameObject.SetActive(false);
        }
        else
        {
            _leftButton.gameObject.SetActive(true);
        }
        if ((_page + 1) * _buildingInfo.Count >= _buildingList.Count)
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
        if (_buildingInfo.Count < _buildingList.Count - _page * _buildingInfo.Count)
        {
            count = _buildingInfo.Count;
        }
        else
        {
            count = _buildingList.Count - _page * _buildingInfo.Count;
        }
        int i = 0;
        for (; i < count; i++)
        {
            BuildingName buildingName = _buildingList[_page* _buildingInfo.Count + i];
            Building buildingOrigin = Manager.Data.GetBuilding(buildingName);

            _buildingInfo[i].buildingName = buildingName;
            _buildingInfo[i].buildingSprite = buildingOrigin.GetSpriteRendererList()[0].sprite;
            _buildingInfo[i].image.sprite = _buildingInfo[i].buildingSprite;
            _buildingInfo[i].image.rectTransform.sizeDelta = Util.CalcFitSize(70, _buildingInfo[i].image.sprite);
            _buildingInfo[i].image.transform.parent.gameObject.SetActive(true);
        }
        for(; i < _buildingInfo.Count; i++)
        {
            _buildingInfo[i].image.transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnUIMouseDown(List<GameObject> list)
    {
        if (_isOpen)
        {
            UIDescription.Close();
        }
        foreach (var info in _buildingInfo)
        {
            if(list.Contains(info.image.gameObject))
            {
                Manager.Building.StartBuildingDraw(Manager.Character.MainCharacter.gameObject,info.buildingName);
                return;
            }
        }
    }
    private void OnMouse1Down(List<GameObject> list)
    {
        foreach (var info in _buildingInfo)
        {
            if (list.Contains(info.image.gameObject))
            {
                UIDescription.OpenBuildingDescription(Manager.Data.GetBuilding(info.buildingName));
                _isOpen = true;
                return;
            }
        }
        if (_isOpen)
        {
            UIDescription.Close();
        }
    }
}
