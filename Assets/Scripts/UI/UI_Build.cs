using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Build : UIBase
{
    [System.Serializable]
    struct BuildingNameSprite
    {
        public BuildingName buildingName;
        public Sprite buildingSprite;
        public Image image;
    }

    [SerializeField]List<BuildingNameSprite> _buildingInfo = new List<BuildingNameSprite>();

    UI_Description _uiDescription = null;
    UI_Description UIDescription { get
        {
            if (_uiDescription == null)
            {
                _uiDescription = Manager.UI.GetUI(UIName.Description) as UI_Description;
            } return _uiDescription; } }
    bool _isOpen=false;
    public override void Init()
    {
        Manager.Input.UIMouseDownHandler += OnUIMouseDown;
        Manager.Input.UIMouse1DownHandler += OnMouse1Down;

        _isDone = true;
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
