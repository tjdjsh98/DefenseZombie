using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelperAI : MonoBehaviour
{
    Character _character;

    Character _target;

    [SerializeField] Vector3 _mainPoint;
    [SerializeField] float _aroundRange;
    [SerializeField] Range _searchRange;
    [SerializeField]Range _attackRange;
    Range AttackRange
    {
        get
        {
            Range temp = _attackRange;
            temp.center.x = transform.localScale.x > 0 ? temp.center.x : -temp.center.x;

            return temp;
        }
    }
    Range SearchRange
    {
        get
        {
            Range temp = _searchRange;
            temp.center.x = transform.localScale.x > 0 ? temp.center.x : -temp.center.x;

            return temp;
        }
    }

    float _time = 0;
    float _searchRenewTime = 1f;

    float _idleDuration = 3;
    float _idleElapsed = 0;
    Vector3 _movePoint;

    public Action AttackHanlder;

    bool _mainPointAlter;

    protected void Awake()
    {
        _character = GetComponent<Character>();
        if (Client.Instance.ClientId != -1)
        {
            StartCoroutine(CorSendMovePacket());
        }
        _movePoint = _mainPoint + Vector3.right * Random.Range(-_aroundRange, _aroundRange);
    }
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + SearchRange.center, SearchRange.size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + AttackRange.center, AttackRange.size);
    }
    private void Update()
    {
        AI();
    }

    public void SetMainPos(float x)
    {
        _mainPoint.x = x;
        _mainPointAlter = true;
    }

    void AI()
    {
        if (_target != null && !_target.gameObject.activeSelf) _target = null;
        if (_character.IsAttacking) return;
        _time += Time.deltaTime;
        if (_time >= _searchRenewTime)
        {
            _time = 0;
            Search();
        }
        if (_character.CharacterState == CharacterState.Idle)
        {
            if(_mainPointAlter)
            {
                _movePoint = _mainPoint;
                _mainPointAlter = false;
            }
            if (_target == null)
            {
                if(Mathf.Abs(_movePoint.x - transform.position.x) <= 0.1f)
                {
                    _character.SetCharacterDirection(Vector3.zero);
                    _idleElapsed += Time.deltaTime;
                    if(_idleElapsed >= _idleDuration)
                    {
                        _movePoint = _mainPoint + Vector3.right * Random.Range(-_aroundRange, _aroundRange);
                        _idleElapsed = 0;
                    }
                }
                else
                {
                    _character.SetCharacterDirection(_movePoint - transform.position);
                }
            }
            else
            {
                if (Util.GetIsInRange(_character.gameObject,_target.gameObject,AttackRange))
                {
                    _character.SetCharacterDirection(Vector3.zero);
                    AttackHanlder?.Invoke();
                    Client.Instance.SendMove(_character);
                }
                else
                {
                    _character.SetCharacterDirection(_target.transform.position - transform.position);
                }
            }
        }
        else
        {
            _character.SetCharacterDirection(Vector2.zero);
        }
    }


    void Search()
    {
        int layerMask = LayerMask.GetMask("Character");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + SearchRange.center, SearchRange.size, 0, Vector2.zero, 0, layerMask);
        if (hits.Length <= 0) return;

        // 가장 근접한 적을 우선 타겟으로 둔다
        float distance = 100;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == _character.gameObject) continue;
            if (hit.collider.gameObject.tag == "Enemy")
            {
                Character character = hit.collider.gameObject.GetComponentInParent<Character>();
                if (character != null && (transform.position - character.transform.position).magnitude < distance)
                {
                    _target = character;
                }
            }
        }
    }

    IEnumerator CorSendMovePacket()
    {
        while (true)
        {
            Client.Instance.SendMove(_character);
            yield return new WaitForSeconds(Client.SendPacketInterval);
        }
    }
}
