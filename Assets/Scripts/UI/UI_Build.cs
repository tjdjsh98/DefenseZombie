using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_Build : UIBase
{
    public override void Init()
    {
        base.Init();
    }

    public void StartBuild(int buildingIndex)
    {
        Manager.Building.StartBuildingDraw(Manager.Character.MainCharacter.gameObject, (BuildingName)buildingIndex);
    }
}
