using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Building : MonoBehaviour, IHp
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

    public Action<bool> BlueprintChangedHandler;

    bool _isConstuctDone;
    public bool IsConstructDone => _isConstuctDone;

    List<IBuildingOption> _buildingOptionList = new List<IBuildingOption>();

    // 좌표에서 사라지기 전, 오브젝트가 파괴되기 전에 실행됩니다.
    public Action DestroyedHandler;


    //원래 가지고 있는 레이어
    int _originLayer;

    private void Awake()
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

        if (Blueprint.BlueprintItmeList.Count > 0)
        {
            _originLayer = gameObject.layer;
            gameObject.layer = UnconstructedBuildingLayer;
            _time = 0;
            Color color = new Color(1, 1, 1, 0.2f);
            foreach (var s in _spriteRenderers)
                s.color = color;
        }
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
            DestroyedHandler?.Invoke();
            Manager.Building.RemoveBuilding(this);
            Destroy(gameObject);
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
    public bool AddItemToConstruction(ItemName itemName)
    {
        for(int i =0; i < Blueprint.BlueprintItmeList.Count; i++)
        {
            if (Blueprint.BlueprintItmeList[i].name == itemName)
            {
                if (Blueprint.BlueprintItmeList[i].requireCount > Blueprint.BlueprintItmeList[i].currentCount)
                {
                    Blueprint.BlueprintItmeList[i].AddCount();
                    bool isFinish = CheckIsFinishConstruct();
                    BlueprintChangedHandler?.Invoke(isFinish);
                    return true;    
                }
            }
        }

        return false;
    }

    bool CheckIsFinishConstruct()
    {
        bool isSuccess = true;

        for (int i = 0; i < Blueprint.BlueprintItmeList.Count; i++)
        {
            if (Blueprint.BlueprintItmeList[i].requireCount > Blueprint.BlueprintItmeList[i].currentCount)
            {
                isSuccess = false;
                break;
            }
        }

        if(isSuccess)
        {
            foreach (var s in _spriteRenderers)
                s.color = Color.white;
            if (_circleSlider != null)
                _circleSlider.Hide();
            _isConstuctDone = true;
            gameObject.layer = _originLayer;
        }

        return isSuccess;
    }
}
