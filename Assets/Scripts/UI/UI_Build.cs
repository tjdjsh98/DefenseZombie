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


    public override void Init()
    {
        Manager.Input.UIMouseDownHandler += OnUIMouseDown;

        _isDone = true;
    }

    private void OnUIMouseDown(List<GameObject> list)
    {
        foreach(var info in _buildingInfo)
        {
            if(list.Contains(info.image.gameObject))
            {
                Manager.Building.StartBuildingDraw(Manager.Character.MainCharacter.gameObject,info.buildingName);
                return;
            }
        }
    }
}
