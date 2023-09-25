using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IHp
{
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider;

    CircleSlider _circleSlider;
    [field: SerializeField] public int MaxHp { set; get; } 
    [field: SerializeField] public int Hp { set; get; } 
    [SerializeField] BuildingSize _size;
    public BuildingSize BuildingSize => _size;

    [SerializeField] float _buildingTime;
    float _time;

    List<Vector2Int> _coordinate = new List<Vector2Int>();

    bool _done;

    private void Awake()
    {
        _spriteRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        
        _circleSlider = GetComponentInChildren<CircleSlider>();
        Hp = MaxHp;

        gameObject.tag = "Untagged";
        _boxCollider.enabled = false;
        _time = 0;
        Color color = new Color(1, 1, 1, 0.2f);
        _spriteRenderer.color = color;
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
            _spriteRenderer.color = Color.white;
            gameObject.tag = "Building";
            _boxCollider.enabled = true;
            _circleSlider.Hide();
            _done = true;   
        }
        else
        {
            _time += Time.deltaTime;
            _circleSlider.SetRatio(_time / _buildingTime);
        }

    }

    public void Damage(int dmg)
    {
        Hp -= dmg;
        if(Hp <=0)
        {
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
}
