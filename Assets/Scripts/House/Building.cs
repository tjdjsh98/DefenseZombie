using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] BuildingSize _size;
    public BuildingSize BuildingSize => _size;

    private void OnDrawGizmosSelected()
    {
     
    }
}
