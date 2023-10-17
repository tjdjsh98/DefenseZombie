using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class BuildingManager : MonoBehaviour
{
    BuildingName _drawingBuildingName;
    GameObject _blankBox;
    List<SpriteRenderer> _blankBoxList = new List<SpriteRenderer>();
    GameObject _builder;

    bool _isDrawing = false;
    public bool IsDrawing => _isDrawing;

    static int _buildingId;

    Dictionary<int, Building> _buildingDictionary = new Dictionary<int, Building>();

    Dictionary<int,Dictionary<int,Building>> _buildingCoordiate = new Dictionary<int, Dictionary<int, Building>>();

    public Action<int, Building> ReciveGenPacket;

    public void Init()
    {
        _blankBox = Resources.Load<GameObject>("BlankBox");

        GenerateBuilding(BuildingName.CommandCenter, new Vector2Int(0, -3));
        
    }

    private void Update()
    {
        if(IsDrawing)
        {
            Building darwBuilding = Manager.Data.GetBuilding(_drawingBuildingName);
            int drawCount = 0;
            Vector3Int fixedPos = Vector3Int.zero;
            fixedPos.x = Mathf.RoundToInt(_builder.transform.position.x) +  (_builder.transform.localScale.x > 0 ? 1 : -1);
            fixedPos.y = Mathf.CeilToInt(_builder.transform.position.y);
            for (int i =0; i < darwBuilding.BuildingSize.width * darwBuilding.BuildingSize.height;i++)
            {
                if (!darwBuilding.BuildingSize.isPlace[i]) continue;

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

                Vector3Int pos = fixedPos + new Vector3Int(i % darwBuilding.BuildingSize.width * (_builder.transform.localScale.x > 0 ? 1 : -1), i / darwBuilding.BuildingSize.width, 0);
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
    public Building GenerateBuilding(BuildingName name, Vector2Int cellPos)
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
        building.BuildingId = ++_buildingId;
        Vector3 buildingPos = new Vector3(cellPos.x + (buildingOrigin.BuildingSize.width - 1) / 2f - 0.5f, cellPos.y - 1f);
        Vector3 scale = Vector3.one;
        if (_builder != null)
        {
            scale.x = _builder.transform.localScale.x > 0 ? 1 : -1;
            building.transform.localScale = scale;
        }
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

    public int PlayerRequestBuilding()
    {
        if (!IsDrawing) return -1;

        Vector2Int cellPos = Vector2Int.zero;
        cellPos.x = Mathf.RoundToInt(_builder.transform.position.x + (_builder.transform.localScale.x > 0 ? 1 : -1));
        cellPos.y = Mathf.CeilToInt(_builder.transform.position.y);

        if (Client.Instance.ClientId != -1)
            return RequestGeneratingBuilding(_drawingBuildingName, cellPos);
        else
            GenerateBuilding(_drawingBuildingName, cellPos);

        return -1;
    }

    public bool SetBuilding(Vector3 initPos, Building building)
    {
        if(building== null) return false;   
        Vector2Int cellPos = Vector2Int.zero;
        cellPos.x = Mathf.RoundToInt(initPos.x);
        cellPos.y = Mathf.CeilToInt(initPos.y);

        // 겹치는 곳이 있는지 확인
        for (int i = 0; i < building.BuildingSize.width * building.BuildingSize.height; i++)
        {
            if (!building.BuildingSize.isPlace[i]) continue;

            Vector2Int pos = cellPos + new Vector2Int(i % building.BuildingSize.width, i / building.BuildingSize.width);

            if (_buildingCoordiate.ContainsKey(pos.x))
            {
                if (_buildingCoordiate[pos.x].ContainsKey(pos.y))
                {
                    if (_buildingCoordiate[pos.x][pos.y] != null)
                        return false;
                }
            }
        }

        // 겹치는 부분에 좌표 설정
        for (int i = 0; i < building.BuildingSize.width * building.BuildingSize.height; i++)
        {
            if (!building.BuildingSize.isPlace[i]) continue;

            Vector2Int pos = cellPos + new Vector2Int(i % building.BuildingSize.width, i / building.BuildingSize.width);

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

        building.transform.parent = transform.parent;
        building.transform.position = new Vector3(cellPos.x + (building.BuildingSize.width - 1) / 2f - 0.5f, cellPos.y - 1f);
        return true;
    }
    public int RequestGeneratingBuilding(BuildingName name, Vector2Int cellPos)
    {
        int requestNumber = UnityEngine.Random.Range(100, 1000);

        Client.Instance.SendRequestGeneratingBuilding(name, cellPos, requestNumber);

        return requestNumber;
    }

    // 서버에서 받은 패킷으로 빌딩 설치
    public bool GenerateBuilding(S_BroadcastGenerateBuilding packet)
    {
        bool isSucess = packet.isSuccess;

        Building building = null;
        if (isSucess)
        {
            BuildingName name = (BuildingName)packet.buildingName;
            Vector2Int cellPos = new Vector2Int(packet.posX, packet.posY);

            Building buildingOrigin = Manager.Data.GetBuilding(name);
            if (buildingOrigin == null)
                isSucess = false;

            building = Instantiate(buildingOrigin);
            building.BuildingId = packet.buildingId;
            Vector3 buildingPos = new Vector3(cellPos.x + (buildingOrigin.BuildingSize.width - 1) / 2f - 0.5f, cellPos.y - 1f);
            building.transform.position = buildingPos;

            // 겹치는 부분에 좌표 설정
            for (int i = 0; i < buildingOrigin.BuildingSize.width * buildingOrigin.BuildingSize.height; i++)
            {
                if (!buildingOrigin.BuildingSize.isPlace[i]) continue;

                Vector2Int pos = cellPos + new Vector2Int(i % buildingOrigin.BuildingSize.width, i / buildingOrigin.BuildingSize.width);


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
        }

        ReciveGenPacket?.Invoke(packet.requestNumber, building);
        return isSucess;
    }

    public bool StartBuildingDraw(GameObject builder, BuildingName name)
    {
        _builder = builder;
        _drawingBuildingName =name;
        _isDrawing = true;

        return true;
    }

    public void StopBuildingDrawing()
    {
        _builder = null;
      
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
        building.ClearCoordinate();
    }
}