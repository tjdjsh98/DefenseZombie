using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequireItemBuildingOption : MonoBehaviour, IBuildingOption
{
    Building _building;

    public void Init()
    {
        _building = GetComponent<Building>();

    }
}
