using System.Collections;
using UnityEngine;
using static Define;

public class EnemyAI : MonoBehaviour, ICharacterOption
{
    protected Character _character;
    protected AnimatorHandler _animatorHandler;
    [SerializeField] protected Weapon _weapon;

    [SerializeField] protected Range _attackRange;
    [SerializeField] protected Range _searchRange;
    protected float _movePacketDelay = 0.25f;

    protected Character _target;

    protected Range AttackRange
    {
        get
        {
            Range range;
            if (_weapon == null)
                range = _attackRange;
            else
                range = _weapon.WeaponAttackData.attackRange;

            range.center.x = transform.localScale.x > 0 ? range.center.x : -range.center.x;

            return range;
            
        }
    }
    protected Range SearchRange
    {
        get
        {
            Range temp = _searchRange;
            temp.center.x = transform.localScale.x > 0 ? temp.center.x : -temp.center.x;

            return temp;
        }
    }

    [SerializeField] protected float _attackDelay;
    protected float _attackTime;

    public bool IsDone { get; set; }


    public virtual void Init()
    {
        _character = GetComponent<Character>();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _weapon = GetComponent<Weapon>();
        if (Client.Instance.ClientId != -1)
        {
            StartCoroutine(CorSendMovePacket());
        }
        _animatorHandler.AttackEndHandler += () => { _character.IsEnableMove = true; };

        IsDone = true;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + SearchRange.center, SearchRange.size);
    }

    private void Update()
    {
        if (!IsDone) return;
        AI();
    }

    protected virtual void AI()
    {
        if (!Client.Instance.IsMain && Client.Instance.ClientId != -1) return;

        if (_target != null && !_target.gameObject.activeSelf) _target = null;
        if (_character.IsAttacking) return;

        if (_character.IsDamaged)
        {
            _character.SetCharacterDirection(Vector2.zero);
            return;
        }

        _attackTime += Time.deltaTime;
        if (_target == null)
        {
            Search();
        }
        else
        {

            Character character = Util.GetGameObjectByPhysics<Character>(transform.position, AttackRange, Define.PlayerLayerMask);
            Building building = Util.GetGameObjectByPhysics<Building>(transform.position, AttackRange, Define.BuildingLayerMask);

            if (character != null || building != null)
            {
                _character.StopMove();
                _character.SetCharacterDirection(Vector2.zero);

                

                if (_attackDelay < _attackTime)
                {
                   
                    _attackTime = 0;
                  
                    _character.IsAttacking = true;
                    _character.IsEnableMove = false;
                    Client.Instance.SendCharacterControlInfo(_character);
                    
                }

                return;
            }
            _character.SetCharacterDirection(_target.transform.position - transform.position);
            _character.Turn((_target.transform.position - transform.position).x);
        }
      
    }


    protected void Search()
    {
        int layerMask = LayerMask.GetMask("Player");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + SearchRange.center, SearchRange.size, 0, Vector2.zero, 0, layerMask);
        if (hits.Length <= 0) return;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == _character.gameObject) continue;
            if (hit.collider.gameObject.tag == "Player")
            {
                Character character = hit.collider.gameObject.GetComponentInParent<Character>();
                if (character != null)
                {
                    _target = character;
                    break;
                }
            }
        }
    }

    protected IEnumerator CorSendMovePacket()
    {
        while (true)
        {
            if (Client.Instance.IsMain)
            {
                Client.Instance.SendCharacterInfo(_character);
                Client.Instance.SendCharacterControlInfo(_character);
            }
            yield return new WaitForSeconds(Client.SendPacketInterval);
        }
    }

    public void DataSerialize()
    {
    }

    public void DataDeserialize()
    {
    }


}
