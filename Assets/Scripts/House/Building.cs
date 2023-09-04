using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;

public class Building : MonoBehaviour
{
    [SerializeField] BuildingSize _size;
    public BuildingSize BuildingSize => _size;

    private void OnDrawGizmosSelected()
    {
        if (_size.isShow)
        {
            for (int i = 0; i < BuildingSize.width * BuildingSize.height; i++)
            {
                if (!BuildingSize.isPlace[i]) continue;

                Color color = new Color();
                color.g = 1;
                color.a = 0.2f;

                Vector3 pos =  new Vector3Int(i % BuildingSize.width * (transform.localScale.x > 0 ? 1 : -1), i / BuildingSize.width, 0);
                pos.x += Mathf.RoundToInt(transform.position.x - (_size.width%2 == 0?0.5f:0));
                pos.x -= (_size.width-1) / 2;
                pos.y += Mathf.RoundToInt(transform.position.y - (_size.height % 2 == 0 ? 0.5f : 0));
                pos.y += 0.5f;

                Gizmos.color = color;
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }

    }
}
