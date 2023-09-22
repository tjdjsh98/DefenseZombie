using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    Building _drawingBuilding;
    GameObject _blankBox;
    List<SpriteRenderer> _blankBoxList = new List<SpriteRenderer>();
    GameObject _builder;

    bool _isDrawing = false;
    public bool IsDrawing => _isDrawing;

    Dictionary<int,Dictionary<int,Building>> _buildingCoordiate = new Dictionary<int, Dictionary<int, Building>>();

    public void Init()
    {
        _blankBox = Resources.Load<GameObject>("BlankBox");

        GenerateBuilding("CommanderHouse", new Vector2Int(0, -3));
        
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
                sr.gameObject.SetActive(true);
                Color color = new Color();
                color.g = 1;
                color.a = 0.2f;

                Vector3Int pos = fixedPos + new Vector3Int(i % _drawingBuilding.BuildingSize.width * (_builder.transform.localScale.x > 0 ? 1 : -1), i / _drawingBuilding.BuildingSize.width, 0);
                if (_buildingCoordiate.ContainsKey(pos.x))
                {
                    if (_buildingCoordiate[pos.x].ContainsKey(pos.y))
                    {
                        if (_buildingCoordiate[pos.x][pos.y] != null)
                        {
                            color.g = 0;
                            color.r = 1;
                        }
                    }
                }

                sr.color = color;
                Vector3 pos2 = pos;
                pos2.x -= 0.5f;
                pos2.y -= 0.5f;
                
                sr.transform.position = pos2;
                drawCount++;
            }

        }
    }

    public Building GenerateBuilding(string name, Vector2Int cellPos)
    {
        Building buildingOrigin = Manager.Data.GetBuilding(name);
        if (buildingOrigin == null) return null;

        // 겹치는 곳이 있는지 확인
        for (int i = 0; i < buildingOrigin.BuildingSize.width * buildingOrigin.BuildingSize.height; i++)
        {
            if (!buildingOrigin.BuildingSize.isPlace[i]) continue;

            Vector2Int pos = cellPos + new Vector2Int(i % buildingOrigin.BuildingSize.width , i / buildingOrigin.BuildingSize.width);

            if (_buildingCoordiate.ContainsKey(pos.x))
            {
                if (_buildingCoordiate[pos.x].ContainsKey(pos.y))
                {
                    if (_buildingCoordiate[pos.x][pos.y] != null)
                        return null;
                }
            }
        }

        Building building = Instantiate(buildingOrigin);
        Vector3 buildingPos = new Vector3(cellPos.x + (buildingOrigin.BuildingSize.width - 1) / 2f - 0.5f, cellPos.y - 1f);


        building.transform.position = buildingPos;


        // 겹치는 부분에 좌표 설정
        for (int i = 0; i < buildingOrigin.BuildingSize.width * buildingOrigin.BuildingSize.height; i++)
        {
            if (!buildingOrigin.BuildingSize.isPlace[i]) continue;

            Vector2Int pos = cellPos + new Vector2Int(i % buildingOrigin.BuildingSize.width , i / buildingOrigin.BuildingSize.width);


            if (!_buildingCoordiate.ContainsKey(pos.x))
            {
                _buildingCoordiate.Add(pos.x, new Dictionary<int, Building>());
            }
            if (!_buildingCoordiate[pos.x].ContainsKey(pos.y))
            {
                _buildingCoordiate[pos.x].Add(pos.y, building);
            }
            else
            {
                _buildingCoordiate[pos.x][pos.y] = building;
            }
            building.AddCoordinate(pos);
        }

        return building;
    }

    public Building GenreateBuilding()
    {
        if (_drawingBuilding == null) return null;

        Vector2Int startPos = Vector2Int.zero;
        startPos.x = Mathf.RoundToInt(_builder.transform.position.x + (_builder.transform.localScale.x > 0 ? 1 : -1));
        startPos.y = Mathf.CeilToInt(_builder.transform.position.y);

        // 겹치는 곳이 있는지 확인
        for (int i = 0; i < _drawingBuilding.BuildingSize.width * _drawingBuilding.BuildingSize.height; i++)
        {
            if (!_drawingBuilding.BuildingSize.isPlace[i]) continue;
           
            Vector2Int pos = startPos + new Vector2Int(i % _drawingBuilding.BuildingSize.width * (_builder.transform.localScale.x > 0 ? 1 : -1), i / _drawingBuilding.BuildingSize.width);

            if (_buildingCoordiate.ContainsKey(pos.x))
            {
                if (_buildingCoordiate[pos.x].ContainsKey(pos.y))
                {
                    if (_buildingCoordiate[pos.x][pos.y] != null)
                        return null;
                }
            }
        }

        Building building = Instantiate(_drawingBuilding);
        Vector3 buildingPos = new Vector3(startPos.x + (_drawingBuilding.BuildingSize.width-1)/2f*(_builder.transform.localScale.x > 0 ? 1 : -1) - 0.5f, startPos.y-1f);

        
        building.transform.position = buildingPos;


        // 겹치는 부분에 좌표 설정
        for (int i = 0; i < _drawingBuilding.BuildingSize.width * _drawingBuilding.BuildingSize.height; i++)
        {
            if (!_drawingBuilding.BuildingSize.isPlace[i]) continue;

            Vector2Int pos = startPos + new Vector2Int(i % _drawingBuilding.BuildingSize.width * (_builder.transform.localScale.x > 0 ? 1 : -1), i / _drawingBuilding.BuildingSize.width);

            
            if (!_buildingCoordiate.ContainsKey(pos.x))
            {
                _buildingCoordiate.Add(pos.x, new Dictionary<int, Building>());
            }
            if (!_buildingCoordiate[pos.x].ContainsKey(pos.y))
            {
                _buildingCoordiate[pos.x].Add(pos.y, building);
            }
            else
            {
                _buildingCoordiate[pos.x][pos.y] = building;
            }
            building.AddCoordinate(pos);
        }
      
        return building;

    }

    public bool StartBuildingDraw(GameObject builder, string name)
    {
        _builder = builder;
        _drawingBuilding = Manager.Data.GetBuilding(name);

        if(_drawingBuilding == null) return false;

        _isDrawing = true;

        return true;
    }

    public void StopBuildingDrawing()
    {
        _builder = null;
        _drawingBuilding = null;
        _isDrawing = false;

        foreach(var blankBox in _blankBoxList)
        {
            blankBox.gameObject.SetActive(false);
        }
    }

    public void RemoveBuilding(Building building)
    {
        List<Vector2Int> list = building.GetCoordinate();
        foreach (var pos in list)
        {
            if(_buildingCoordiate[pos.x][pos.y] == building)
            {
                _buildingCoordiate[pos.x][pos.y] = null;
            }
        }
    }
}