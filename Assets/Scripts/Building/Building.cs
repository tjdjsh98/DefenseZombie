using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Building : MonoBehaviour, IHp, IEnableInsertItem, IDataSerializable
{
    List<SpriteRenderer> _spriteRenderers;
    BoxCollider2D _boxCollider;
    CircleSlider _circleSlider;

    public int BuildingId { set; get; }

    [field: SerializeField] public bool _isTile;
    [field: SerializeField] public Define.BuildingName BuildingName { private set; get; }
    [field: SerializeField] public int MaxHp { set; get; } 
    [field: SerializeField] public int Hp { set; get; } 
    [SerializeField] BuildingSize _size;
    [SerializeField] float _buildingTime;
    [SerializeField][TextArea] string _description;
    public string Description=>_description;
    [SerializeField] BuildingBlueprint _blueprint;
    public BuildingBlueprint Blueprint => _blueprint;

    public BuildingSize BuildingSize => _size;

    float _time;

    List<Vector2Int> _coordinate = new List<Vector2Int>();

    public Action<float> TurnHandler;

    public Action<bool> ItemChangedHandler { get; set; }

    bool _isConstuctDone;
    public bool IsConstructDone => _isConstuctDone;

    List<IBuildingOption> _buildingOptionList = new List<IBuildingOption>();

    // 좌표에서 사라지기 전, 오브젝트가 파괴되기 전에 실행됩니다.
    public Action DestroyedHandler;


    //원래 가지고 있는 레이어
    int _originLayer;

    public bool InitDone { get; private set; } = false; 
    public void Init()
    {
        if(_spriteRenderers == null)
            _spriteRenderers = transform.Find("Model").GetComponentsInChildren<SpriteRenderer>().ToList();
        _boxCollider = GetComponent<BoxCollider2D>();
        
        _buildingOptionList = GetComponents<IBuildingOption>().ToList();
        foreach (var option in _buildingOptionList)
        {
            option.Init();
        }
        Hp = MaxHp;

        _originLayer = gameObject.layer;

        if (Blueprint.BlueprintItemList.Count > 0)
        {
            gameObject.layer = UnconstructedBuildingLayer;
            _time = 0;
            Color color = new Color(1, 1, 1, 0.2f);
            foreach (var s in _spriteRenderers)
                s.color = color;
        }
        else
        {
            _isConstuctDone = true;
        }

        InitDone= true;
    }

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
                pos.x -= (_size.width-1) / 2f;
                pos.y += Mathf.RoundToInt(transform.position.y );
                pos.y += 0.5f;
                Gizmos.color = color;
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
        }
    }

    public void Damage(int dmg)
    {
        Hp -= dmg;
        if(Hp <=0)
        {
            if (Client.Instance.IsSingle || Client.Instance.IsMain)
            {
                DestroyedHandler?.Invoke();
                Manager.Building.RemoveBuilding(BuildingId);
            }
        }
        else
        {
            if(!Client.Instance.IsSingle && Client.Instance.IsMain)
                Client.Instance.SendBuildingInfo(this);
        }
    }

    public void AddCoordinate(Vector2Int pos)
    {
        _coordinate.Add(pos);
    }

    public List<Vector2Int> GetCoordinate()
    {
        return _coordinate;
    }
    public void ClearCoordinate()
    {
        _coordinate.Clear();
    }
    public List<SpriteRenderer> GetSpritesRenderers()
    {
        return transform.Find("Model").GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    // 건축 중인 건물에 재료 아이템을 넣음
    // 건축이 완료된 건물이거나 해당 아이템이 필요없다면 false를 반환
    // 해당 아이템을 추가 완료된다면 아이템을 없애지 않고 내용물에 추가만 합니다.
    public bool InsertItem(Item item)
    {
        bool isSuccess = false;
        if (IsConstructDone) return isSuccess;

        for(int i =0; i < Blueprint.BlueprintItemList.Count; i++)
        {
            if (Blueprint.BlueprintItemList[i].name == item.ItemData.ItemName)
            {
                if (Blueprint.BlueprintItemList[i].requireCount > Blueprint.BlueprintItemList[i].currentCount)
                {
                    Blueprint.BlueprintItemList[i].AddCount(item.ItemId);
                    item.Hide();
                    bool isFinish = CheckIsFinish();
                    ItemChangedHandler?.Invoke(isFinish);
                    isSuccess = true;
                    break;
                }
            }
        }

        if(isSuccess&&Client.Instance.IsMain)
        {
            Client.Instance.SendBuildingInfo(this);
        }

        return isSuccess;
    }

    public bool CheckIsFinish()
    {
        if(_isConstuctDone) return true;
        bool isSuccess = true;

        for (int i = 0; i < Blueprint.BlueprintItemList.Count; i++)
        {
            if (Blueprint.BlueprintItemList[i].requireCount > Blueprint.BlueprintItemList[i].currentCount)
            {
                isSuccess = false;
                break;
            }
        }

        if(isSuccess)
        {
            if (Client.Instance.IsSingle || Client.Instance.IsMain)
            {
                foreach (var blueprint in Blueprint.BlueprintItemList)
                {
                    foreach (var id in blueprint.itemIdList)
                    {
                        Manager.Item.RemoveItem(id);
                    }

                    blueprint.itemIdList.Clear();
                }
            }

            foreach (var s in _spriteRenderers)
                s.color = Color.white;
            if (_circleSlider != null)
                _circleSlider.Hide();
            _isConstuctDone = true;
            gameObject.layer = _originLayer;
            Debug.Log("A");
        }

        return isSuccess;
    }

    public string SerializeData()
    {
        Util.StartWriteSerializedData();

        Util.WriteSerializedData(Blueprint.BlueprintItemList.Count);
        for (int i = 0; i < Blueprint.BlueprintItemList.Count; i++)
        {
            int name = (int)Blueprint.BlueprintItemList[i].name;
            Util.WriteSerializedData(name);
            int requireCount = Blueprint.BlueprintItemList[i].requireCount;
            Util.WriteSerializedData(requireCount);
            int currentCount = Blueprint.BlueprintItemList[i].currentCount;
            Util.WriteSerializedData(currentCount);

            Util.WriteSerializedData(Blueprint.BlueprintItemList[i].itemIdList.Count);
            for (int j = 0; j < Blueprint.BlueprintItemList[i].itemIdList.Count; j++)
            {
                int id =  Blueprint.BlueprintItemList[i].itemIdList[j];
                Util.WriteSerializedData(id);
            }
            
        }

        foreach (var option in _buildingOptionList)
        {
            option.SerializeData();
        }

        return Util.EndWriteSerializeData();
    }

    public void DeserializeData(string stringData)
    {
        if (stringData.Equals(string.Empty)) return;

        Util.StartReadSerializedData(stringData);

        int blueprintItemCount = Util.ReadSerializedDataToInt();
        for(int i = 0; i < blueprintItemCount; i++)
        {
            int name = Util.ReadSerializedDataToInt();
            int requireCount = Util.ReadSerializedDataToInt();
            int currentCount = Util.ReadSerializedDataToInt();

            if (_blueprint.BlueprintItemList.Count <= i)
            {
                _blueprint.BlueprintItemList.Add(new BlueprintItem((ItemName)name, requireCount, currentCount));
            }
            else
            {
                _blueprint.BlueprintItemList[i].name = (ItemName)name;
                _blueprint.BlueprintItemList[i].requireCount = requireCount;
                _blueprint.BlueprintItemList[i].currentCount = currentCount;
            }
            int itemIdCount = Util.ReadSerializedDataToInt();

            _blueprint.BlueprintItemList[i].itemIdList.Clear();

            for (int j = 0; j < itemIdCount; j++)
            {
                int id = Util.ReadSerializedDataToInt();
                if (_blueprint.BlueprintItemList[i].itemIdList.Count >= j)
                    _blueprint.BlueprintItemList[i].itemIdList.Add(id);
                else
                    _blueprint.BlueprintItemList[i].itemIdList[j] = id;
            }
        }
        ItemChangedHandler?.Invoke(CheckIsFinish());

        foreach (var option in _buildingOptionList)
        {
            option.DeserializeData();
        }

    }

    public string SerializeControlData()
    {
        Util.StartWriteSerializedData();

        foreach (var option in _buildingOptionList)
        {
            option.SerializeControlData();
        }

        return Util.EndWriteSerializeData();
    }

    public void DeserializeControlData(string stringData)
    {
        if (stringData == null) return;

        Util.StartReadSerializedData(stringData);

        foreach (var option in _buildingOptionList)
        {
            option.DeserializeControlData();
        }

        return;
    }

    public void SyncBuildingInfo(S_BroadcastBuildingInfo packet)
    {
        if (Client.Instance.IsMain) return;

        if (packet == null) return;

        transform.position = new Vector3(packet.posX,packet.posY);
        Hp = packet.hp;

        DeserializeData(packet.data);
    }
    public void SyncBuildingControlInfo(S_BroadcastBuildingControlInfo packet)
    {
        if (packet == null) return;

        DeserializeControlData(packet.data);
    }

    public List<SpriteRenderer> GetSpriteRendererList()
    {
        if (InitDone == false)
        {
            _spriteRenderers = transform.Find("Model").GetComponentsInChildren<SpriteRenderer>().ToList();
        }
        return _spriteRenderers;
    }

}
