using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public BuildingSize BuildingSize => _size;

    float _time;

    List<Vector2Int> _coordinate = new List<Vector2Int>();

    public Action<float> TurnHandler;

    bool _done;

    // 좌표에서 사라지기 전, 오브젝트가 파괴되기 전에 실행됩니다.
    public Action DestroyHandler;


    private void Awake()
    {
        _spriteRenderers = transform.Find("Model").GetComponentsInChildren<SpriteRenderer>().ToList();
        SpriteRenderer sr = transform.Find("Model").GetComponent<SpriteRenderer>();
        if(sr != null)
            _spriteRenderers.Add(sr);
        _boxCollider = GetComponent<BoxCollider2D>();
        
        _circleSlider = GetComponentInChildren<CircleSlider>();
        Hp = MaxHp;

        gameObject.tag = "Untagged";
        _boxCollider.enabled = false;
        _time = 0;
        Color color = new Color(1, 1, 1, 0.2f);
        foreach (var s in _spriteRenderers)
            s.color = color;
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

    public void Update()
    {
        if(_done) return;

        if(_time > _buildingTime)
        {
            foreach(var s in _spriteRenderers)
                s.color = Color.white;
            gameObject.tag = "Building";
            _boxCollider.enabled = true;
            if(_circleSlider!= null)
                _circleSlider.Hide();
            _done = true;
        }
        else
        {
            _time += Time.deltaTime;
            if (_circleSlider != null)
                _circleSlider.SetRatio(_time / _buildingTime);
        }

    }

    public void Damage(int dmg)
    {
        Hp -= dmg;
        if(Hp <=0)
        {
            DestroyHandler?.Invoke();
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

}
