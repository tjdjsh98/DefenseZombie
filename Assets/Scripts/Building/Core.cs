using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class Core : MonoBehaviour, IBuildingOption
{
    Building _building;

    public void Init()
    {
        _building = GetComponent<Building>();

        _building.DestroyedHandler += OnDestroyed;
    }

    public void DataDeserialize()
    {

    }
    public void DataSerialize()
    {
        
    }

    void OnDestroyed()
    {
        UI_GameOver gameOver = Manager.UI.GetUI(Define.UIName.GameOver) as UI_GameOver;
        gameOver.GameOver();
    }
}
