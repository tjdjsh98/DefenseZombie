using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    Building _drawingBuilding;
    GameObject _blankBox;
    List<SpriteRenderer> _blankBoxList = new List<SpriteRenderer>();
    GameObject _builder;
    
    Dictionary<int,Dictionary<int,Building>> _buildingCoordiate = new Dictionary<int, Dictionary<int, Building>>();

    public void Init()
    {
        _blankBox = Resources.Load<GameObject>("BlankBox");
    }

    private void Update()
    {
        if(_drawingBuilding)
        {
            int drawCount = 0;
            Vector3Int fixedPos = Vector3Int.zero;
            fixedPos.x = Mathf.RoundToInt(_builder.transform.position.x) +  (_builder.transform.localScale.x > 0 ? 1 : -1);
            fixedPos.y = Mathf.CeilToInt(_builder.transform.position.y);
            for (int i =0; i < _drawingBuilding.BuildingSize.width * _drawingBuilding.BuildingSize.height;i++)
            {
                if (!_drawingBuilding.BuildingSize.isPlace[i]) continue;

                SpriteRenderer sr = null;
                if(_blankBoxList.Count <= drawCount)
                {
                    SpriteRenderer temp = Instantiate(_blankBox).GetComponent<SpriteRenderer>();
                    sr = temp;
                    _blankBoxList.Add(sr);
                }
                sr = _blankBoxList[drawCount];
                Color color = new Color();
                color.g = 1;
                color.a = 0.2f;

                Vector3Int pos = fixedPos + new Vector3Int(i % _drawingBuilding.BuildingSize.width * (_builder.transform.localScale.x > 0 ? 1 : -1), i / _drawingBuilding.BuildingSize.width, 0);
                if (_buildingCoordiate.ContainsKey(pos.x))
                {
                    if (_buildingCoordiate[pos.x].ContainsKey(pos.y))
                    {
                        color.g = 0;
                        color.r = 1;
                    }
                }

                sr.color = color;
                sr.transform.position = pos;
                drawCount++;
            }

        }
    }

    public Building GenreateBuilding()
    {
        if (_drawingBuilding == null) return null;

        Vector3Int startPos = Vector3Int.zero;
        startPos.x = Mathf.RoundToInt(_builder.transform.position.x + (_builder.transform.localScale.x > 0 ? 1 : -1));
        startPos.y = Mathf.CeilToInt(_builder.transform.position.y);

        // 겹치는 곳이 있는지 확인
        for (int i = 0; i < _drawingBuilding.BuildingSize.width * _drawingBuilding.BuildingSize.height; i++)
        {
            if (!_drawingBuilding.BuildingSize.isPlace[i]) continue;
           
            Vector3Int pos = startPos + new Vector3Int(i % _drawingBuilding.BuildingSize.width * (_builder.transform.localScale.x > 0 ? 1 : -1), i / _drawingBuilding.BuildingSize.width, 0);

            if (_buildingCoordiate.ContainsKey(pos.x))
            {
                if (_buildingCoordiate[pos.x].ContainsKey(pos.y))
                {
                    return null;
                }
            }
        }

        Building building = Instantiate(_drawingBuilding);
        Vector3 buildingPos = new Vector3(startPos.x + (_drawingBuilding.BuildingSize.width-1)/2f*(_builder.transform.localScale.x > 0 ? 1 : -1), startPos.y-0.5f);

        
        building.transform.position = buildingPos;


        // 겹치는 부분에 좌표 설정
        for (int i = 0; i < _drawingBuilding.BuildingSize.width * _drawingBuilding.BuildingSize.height; i++)
        {
            if (!_drawingBuilding.BuildingSize.isPlace[i]) continue;

            Vector3Int pos = startPos + new Vector3Int(i % _drawingBuilding.BuildingSize.width * (_builder.transform.localScale.x > 0 ? 1 : -1), i / _drawingBuilding.BuildingSize.width, 0);

            
            if (!_buildingCoordiate.ContainsKey(pos.x))
            {
                _buildingCoordiate.Add(pos.x, new Dictionary<int, Building>());
            }
            if (!_buildingCoordiate[pos.x].ContainsKey(pos.y))
            {
                _buildingCoordiate[pos.x].Add(pos.y, building);
            }
        }
      
        return building;

    }

    public bool StartBuildingDraw(GameObject builder, string name)
    {
        _builder = builder;
        _drawingBuilding = Manager.Data.GetBuilding(name);

        if(_drawingBuilding == null) return false;

        return true;
    }

}
