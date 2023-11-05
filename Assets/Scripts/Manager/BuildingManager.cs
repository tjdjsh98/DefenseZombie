using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class BuildingManager : MonoBehaviour
{
    BuildingName _drawingBuildingName;
    GameObject _blankBox;
    List<SpriteRenderer> _previewBuildings = new List<SpriteRenderer>();
    List<SpriteRenderer> _blankBoxList = new List<SpriteRenderer>();
    GameObject _builder;

    bool _isDrawing = false;
    public bool IsDrawing => _isDrawing;

    static int _buildingId;

    Dictionary<int, Building> _buildingDictionary = new Dictionary<int, Building>();

    Dictionary<int,Dictionary<int,Building>> _buildingCoordiate = new Dictionary<int, Dictionary<int, Building>>();

    public Action<int, Building> ReciveGenPacket;

    GameObject _tileFolder;

    public Action<int> ReciveRemoveBuildingPacketHandler;
    public void Init()
    {
        _blankBox = Resources.Load<GameObject>("BlankBox");
        _tileFolder = GameObject.Find("TileFolder");
        if (_tileFolder.gameObject == null)
        {
            _tileFolder = new GameObject("TileFolder");
            _tileFolder.AddComponent<CompositeCollider2D>();
        }
        if (!Client.Instance.IsSingle && Client.Instance.IsMain)
        {
            StartCoroutine(SendPacketCor());
        }

    }

    private void Update()
    {
        if(IsDrawing)
        {
            Building darwBuilding = Manager.Data.GetBuilding(_drawingBuildingName);
            int drawCount = 0;
            Vector3Int cellPos = Vector3Int.zero;
            cellPos.x = (_builder.transform.localScale.x > 0 ? Mathf.CeilToInt(_builder.transform.position.x + 0.5f) : Mathf.FloorToInt(_builder.transform.position.x - 0.5f));
            if (Manager.Character.MainCharacter.transform.localScale.x < 0)
                cellPos.x -= darwBuilding.BuildingSize.width - 1;
            cellPos.y = Mathf.CeilToInt(_builder.transform.position.y);

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
                color.a = 0.9f;

                Vector3Int pos = cellPos + new Vector3Int(i % darwBuilding.BuildingSize.width , i / darwBuilding.BuildingSize.width, 0);
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
                pos2.y -= 0.5f;
                
                sr.transform.position = pos2;
                drawCount++;
            }

            Vector3 buildingPos = new Vector3(cellPos.x, cellPos.y - 1f);
            buildingPos.x += (darwBuilding.BuildingSize.width) / 2f - 0.5f;
            SetPreviewBuildingPos(buildingPos);

        }
    }

    public int PlayerRequestBuilding()
    {
        if (!IsDrawing) return -1;

        Building darwBuilding = Manager.Data.GetBuilding(_drawingBuildingName);
        Vector2Int cellPos = Vector2Int.zero;
        cellPos.x = (_builder.transform.localScale.x > 0 ? Mathf.CeilToInt(_builder.transform.position.x + 0.5f) : Mathf.FloorToInt(_builder.transform.position.x - 0.5f));
        if(Manager.Character.MainCharacter.transform.localScale.x < 0)
                cellPos.x -= darwBuilding.BuildingSize.width - 1;
        cellPos.y = Mathf.CeilToInt(_builder.transform.position.y);


        Building building = null;

        return GenerateBuilding(_drawingBuildingName, cellPos, ref building);

    }

    public bool SetBuilding(GameObject builder, Vector3 initPos, Building building)
    {
        if(building== null) return false;   
        Vector2Int cellPos = Vector2Int.zero;
        cellPos.x = (builder.transform.localScale.x > 0 ? Mathf.CeilToInt(builder.transform.position.x + 0.5f) : Mathf.FloorToInt(builder.transform.position.x - 0.5f));
        cellPos.y = Mathf.CeilToInt(builder.transform.position.y);

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

        Vector3 buildingPos = new Vector3(cellPos.x, cellPos.y - 1f);
        buildingPos.x += (building.BuildingSize.width) / 2f - 0.5f;

        building.transform.position = buildingPos;
        return true;
    }

    // 혼용으로 건물 생성
    public int GenerateBuilding(BuildingName name, Vector2Int cellPos, ref Building building)
    {
        int requestNumber = -1;
        if(Client.Instance.IsSingle)
        {
            building = GenerateBuilding(name, cellPos);
        }
        else
        {
            requestNumber = RequestGeneratingBuilding(name, cellPos);
        }

        return requestNumber;
    }

    // 멀티 일 때 서버에 건물 생성을 요청
    private int RequestGeneratingBuilding(BuildingName name, Vector2Int cellPos)
    {
        int requestNumber = UnityEngine.Random.Range(100, 1000);

        Client.Instance.SendRequestGeneratingBuilding(name, cellPos, requestNumber);

        return requestNumber;
    }

    // 싱글 일 때 건물을 생성
    private Building GenerateBuilding(BuildingName name, Vector2Int cellPos)
    {
        Building buildingOrigin = Manager.Data.GetBuilding(name);
        if (buildingOrigin == null) return null;

        // 겹치는 곳이 있는지 확인
        for (int i = 0; i < buildingOrigin.BuildingSize.width * buildingOrigin.BuildingSize.height; i++)
        {
            if (!buildingOrigin.BuildingSize.isPlace[i]) continue;

            Vector2Int pos = cellPos + new Vector2Int(i % buildingOrigin.BuildingSize.width, i / buildingOrigin.BuildingSize.width);

            if (_buildingCoordiate.ContainsKey(pos.x))
            {
                if (_buildingCoordiate[pos.x].ContainsKey(pos.y))
                {
                    if (_buildingCoordiate[pos.x][pos.y] != null)
                        return null;
                }
            }
        }

        // 건물 생성
        Building building = Instantiate(buildingOrigin,_tileFolder.transform);
        if (building._isTile)
        {
            building.transform.parent = _tileFolder.transform;
        }
        building.BuildingId = ++_buildingId;
        Vector3 buildingPos = new Vector3(cellPos.x, cellPos.y - 1f);
        buildingPos.x += (building.BuildingSize.width) / 2f - 0.5f;
        building.transform.position = buildingPos;

        building.Init();

        _buildingDictionary.Add(building.BuildingId, building);


        // 생성 좌표 입력
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

        return building;
    }


    // 혼용으로 건물 삭제
    public int RemoveBuilding(int buildingId)
    {
        int requsetNumber = -1;
        if(Client.Instance.IsSingle)
        {
            SingleRemoveBuilding(buildingId);
        }
        else
        {
            requsetNumber = RequestRemoveBuilding(buildingId);
        }

        return requsetNumber;
    }

    // 멀티일 때 서버에 건물 삭제 요청
    private int RequestRemoveBuilding(int buildingId)
    {
        int requestNumber = Random.Range(10, 1000);

        Client.Instance.SendRequestRemoveBuilding(buildingId, requestNumber);

        return requestNumber;
    }

    // 싱글일 떄 건물 삭제
    private void SingleRemoveBuilding(int buildingId)
    {
        Building building = null;

        if(_buildingDictionary.TryGetValue(buildingId, out building))
        {
            RemoveBuildingCoordinate(building);

            _buildingDictionary.Remove(buildingId);
            Destroy(building.gameObject);
        }
    }


    // 좌표만 삭제
    public void RemoveBuildingCoordinate(Building building)
    {
        List<Vector2Int> list = building.GetCoordinate();
        foreach (var pos in list)
        {
            if (_buildingCoordiate[pos.x][pos.y] == building)
            {
                _buildingCoordiate[pos.x][pos.y] = null;
            }
        }
        building.ClearCoordinate();
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

            building = Instantiate(buildingOrigin, _tileFolder.transform);
            building.BuildingId = packet.buildingId;
            Vector3 buildingPos = new Vector3(cellPos.x, cellPos.y - 1f);
            buildingPos.x += (building.BuildingSize.width) / 2f - 0.5f;
            building.transform.position = buildingPos;

            Debug.Log(building.transform.position);

            building.Init();

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

            _buildingDictionary.Add(building.BuildingId, building);
        }

        ReciveGenPacket?.Invoke(packet.requestNumber, building);
        return isSucess;
    }

    public void GenerateBuildingByPacket(S_EnterSyncInfos packet)
    {
        foreach (var info in packet.buildingInfos)
        {
            BuildingName name = (BuildingName)info.buildingName;
            Vector2Int cellPos = new Vector2Int(info.cellPosX, info.cellPosY);

            Building buildingOrigin = Manager.Data.GetBuilding(name);
            if (buildingOrigin == null) continue;

            if(_buildingDictionary.ContainsKey(info.buildingId))
            {
                Destroy(_buildingDictionary[info.buildingId]);
                _buildingDictionary.Remove(info.buildingId);
            }

            Building building = null;
            building = Instantiate(buildingOrigin, _tileFolder.transform);
            building.BuildingId = info.buildingId;
            Vector3 buildingPos = new Vector3(cellPos.x, cellPos.y - 1f);
            buildingPos.x += (building.BuildingSize.width) / 2f - 0.5f;
            building.transform.position = buildingPos;

            building.Init();

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

            _buildingDictionary.Add(building.BuildingId,building);
        }
    }


    // 받은 패킷으로 건물 삭제
    public void RemoveBuildingByPacket(S_BroadcastRemoveBuilding packet)
    {
        int id = packet.buildingId;
        Building building = null;
        if(_buildingDictionary.TryGetValue(id,out building))
        {
            SingleRemoveBuilding(id);

            ReciveRemoveBuildingPacketHandler?.Invoke(packet.requestNumber);
        }
    }

    public bool StartBuildingDraw(GameObject builder, BuildingName name)
    {
        _builder = builder;
        _drawingBuildingName =name;
        _isDrawing = true;

        ShowPreviewBuilding();
        foreach (var blankBox in _blankBoxList)
        {
            blankBox.gameObject.SetActive(false);
        }
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
        HidePreviewBuilding();
    }

 

    public bool GetIsExistBuilding(Vector2Int cellPos)
    {
        if (!_buildingCoordiate.ContainsKey(cellPos.x)) return false;
        if (!_buildingCoordiate[cellPos.x].ContainsKey(cellPos.y)) return false;

        return _buildingCoordiate[cellPos.x][cellPos.y] != null;
    }

    public Building GetBuilding(int id)
    {
        Building result = null;
        if(_buildingDictionary.TryGetValue(id,out result))
            return result;

        return null;
    }

    void ShowPreviewBuilding()
    {
        Building building = Manager.Data.GetBuilding(_drawingBuildingName);
        if (building == null) return;

        List<SpriteRenderer> spriteRenderers = building.GetSpritesRenderers();
        int i = 0;
        for (; i < spriteRenderers.Count; i++)
        {
            if(_previewBuildings.Count <= i)
            {
                SpriteRenderer sr = Instantiate(Resources.Load<SpriteRenderer>("PreviewSprite"));

                _previewBuildings.Add(sr);
            }
            _previewBuildings[i].sprite = spriteRenderers[i].sprite;
            _previewBuildings[i].color = new Color(1, 1, 1, 0.7f);
            _previewBuildings[i].gameObject.SetActive(true);
        }

        for (; i < _previewBuildings.Count; i++)
        {
            _previewBuildings[i].gameObject.SetActive(false);
        }
    }

    void HidePreviewBuilding()
    {
        foreach(var sr in _previewBuildings)
            sr.gameObject.SetActive(false);
    }

    void SetPreviewBuildingPos(Vector3 pos)
    {
        foreach(var sr in _previewBuildings)
        {
            sr.transform.position = pos;
        }
    }

    // 타일인데 위쪽이 해당 칸 만큼 비어있다면 반환해줍니다.
    // 해당하는 타일이 없다면 (-999, -999)를 반환합니다.
    public Vector2Int FindRandomEmptyGroundInRange(BuildingSize size)
    {
        List<Building> buildingList = _buildingDictionary.Values.ToList();
        buildingList.Shuffle();
        foreach(var building in buildingList)
        {
            bool _isSuccess = true;
            if(building._isTile)
            {
                Vector2Int tempCell = building.GetCoordinate()[0];

                for(int w = 0; w < size.width ; w++)
                {
                    for(int h = 0; h < size.height ; h++)
                    {
                        if (GetIsExistBuilding(tempCell + new Vector2Int(w,h+1)))
                        {
                            _isSuccess = false;
                            break;
                        }
                    }
                    if (!_isSuccess) break;
                }

                if (_isSuccess)
                    return tempCell + Vector2Int.up;
            }
        }

        return Vector2Int.one * -999;
    }

    IEnumerator SendPacketCor()
    {
        while (true)
        {
            foreach (var building in _buildingDictionary.Values)
            {
                if(!building._isTile && building.InitDone) 
                    Client.Instance.SendBuildingInfo(building);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}