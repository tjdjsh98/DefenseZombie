using System;
using System.Collections.Generic;
using System.Linq;
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

    // ��ǥ���� ������� ��, ������Ʈ�� �ı��Ǳ� ���� ����˴ϴ�.
    public Action DestroyedHandler;


    //���� ������ �ִ� ���̾�
    int _originLayer;

    public bool InitDone { get; private set; } = false; 
    public void Init()
    {
        _spriteRenderers = transform.Find("Model").GetComponentsInChildren<SpriteRenderer>().ToList();
        SpriteRenderer sr = transform.Find("Model").GetComponent<SpriteRenderer>();
        if(sr != null)
            _spriteRenderers.Add(sr);
        _boxCollider = GetComponent<BoxCollider2D>();
        
        _buildingOptionList = GetComponents<IBuildingOption>().ToList();
        foreach(var option in _buildingOptionList)
            option.Init();
        Hp = MaxHp;

        if (Blueprint.BlueprintItemList.Count > 0)
        {
            _originLayer = gameObject.layer;
            gameObject.layer = UnconstructedBuildingLayer;
            _time = 0;
            Color color = new Color(1, 1, 1, 0.2f);
            foreach (var s in _spriteRenderers)
                s.color = color;
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

    // ���� ���� �ǹ��� ��� �������� ����
    // ������ �Ϸ�� �ǹ��̰ų� �ش� �������� �ʿ���ٸ� false�� ��ȯ
    // �ش� �������� �߰� �Ϸ�ȴٸ� �������� ������ �ʰ� ���빰�� �߰��� �մϴ�.
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
    }

    public void SyncBuildingInfo(S_BroadcastBuildingInfo packet)
    {
        if (packet == null) return;

        transform.position = new Vector3(packet.posX,packet.posY);
        Hp = packet.hp;

        DeserializeData(packet.data);
    }
}
