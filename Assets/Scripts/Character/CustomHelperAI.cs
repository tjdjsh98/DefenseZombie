using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomHelperAI : HelperAI
{
    CustomCharacter _customCharacter;
    Weapon _weapon;
    protected override Range AttackRange
    {
        get
        {
            Range range;
            if (_weapon == null)
                range = _attackRange;
            else
                range = _weapon.AttackData.attackRange;

            range.center.x = transform.localScale.x > 0 ? range.center.x : -range.center.x;

            return range;

        }
    }

    [SerializeField] bool _isPickItem;
    Item _targetItem;

    public override void Init()
    {
        base.Init();
        _weapon = GetComponent<Weapon>();
        _customCharacter = _character as CustomCharacter;
    }

    protected override void AI()
    {
        if (_target != null && !_target.gameObject.activeSelf) _target = null;
        if (_character.IsAttacking) return;
        _time += Time.deltaTime;

        if (_isPickItem)
        {
            PickItemAI();
        }
        else
        {
            if (_time >= _searchRenewTime)
            {
                _time = 0;
                Search();
            }
            if (_character.IsDamaged)
            {
                _character.SetCharacterDirection(Vector2.zero);
                return;
            }

            if (_mainPointAlter)
            {
                _movePoint = _mainPoint;
                _mainPointAlter = false;
            }

            if (_target == null)
            {
                if (Mathf.Abs(_movePoint.x - transform.position.x) <= 0.1f)
                {
                    _character.SetCharacterDirection(Vector3.zero);
                    _idleElapsed += Time.deltaTime;
                    if (_idleElapsed >= _idleDuration)
                    {
                        _movePoint = _mainPoint + Vector3.right * Random.Range(-_aroundRange, _aroundRange);
                        _idleElapsed = 0;
                    }
                }
                else
                {
                    _character.Turn((_movePoint - transform.position).x);
                    _character.SetCharacterDirection(_movePoint - transform.position);

                }
            }
            else
            {
                Vector3 targetPos = _target.transform.position;
                targetPos += _target.CharacterSize.center;

                _character.Turn((targetPos - transform.position).x);
                _customCharacter.RotationHand(targetPos);

                Range range = AttackRange;
                range.center /= 2;
                if (Util.GetIsInRange(_character.gameObject, _target.gameObject, range))
                {
                    _character.SetCharacterDirection(Vector3.zero);
                    AttackHanlder?.Invoke();
                    Client.Instance.SendCharacterControlInfo(_character);
                }
                else
                {
                    _character.SetCharacterDirection(_target.transform.position - transform.position);
                }
            }

        }
    }

    void PickItemAI()
    {
        if (_targetItem != null && _targetItem.IsFreeze)
            _targetItem = null;

        if (_time >= _searchRenewTime)
        {
            SearchItem();
        }

        if (_character.IsDamaged && _character.IsAttacking)
        {
            _character.SetCharacterDirection(Vector2.zero);
            return;
        }
        if (_mainPointAlter)
        {
            _movePoint = _mainPoint;
            _mainPointAlter = false;
        }

        if (_customCharacter.HoldingItem == null && _customCharacter.WeaponData.WeaponName == Define.WeaponName.None)
        {

            if (_targetItem == null)
            {
                if (Mathf.Abs(_movePoint.x - transform.position.x) <= 0.1f)
                {
                    _character.SetCharacterDirection(Vector3.zero);
                    _idleElapsed += Time.deltaTime;
                    if (_idleElapsed >= _idleDuration)
                    {
                        _movePoint = _mainPoint + Vector3.right * Random.Range(-_aroundRange, _aroundRange);
                        _idleElapsed = 0;
                    }
                }
                else
                {
                    _character.Turn((_movePoint - transform.position).x);
                    _character.SetCharacterDirection(_movePoint - transform.position);

                }
            }
            else
            {
                if (Mathf.Abs(_targetItem.transform.position.x - transform.position.x) > 0.2f)
                {
                    _character.Turn((_targetItem.transform.position - transform.position).x);
                    _customCharacter.SetCharacterDirection(_targetItem.transform.position - transform.position);
                }
                else
                {
                    _customCharacter.GrapItem(_targetItem);
                }
            }
        }
        else
        {
            if (Mathf.Abs(_mainPoint.x - transform.position.x) > 0.2f)
            {
                _customCharacter.Turn((_mainPoint - transform.position).x);
                _customCharacter.SetCharacterDirection(_mainPoint - transform.position);
            }
            else
            {
                _customCharacter.Putdown();
            }

        }
    }

    protected void SearchItem()
    {
        int layerMask = Define.ItemLayerMask;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + SearchRange.center, SearchRange.size, 0, Vector2.zero, 0, layerMask);
        if (hits.Length <= 0) return;

        bool isSearch = false;

        // 가장 근접한 적을 우선 타겟으로 둔다
        float distance = 100;
        foreach (RaycastHit2D hit in hits)
        {
            if (Mathf.Abs(hit.collider.gameObject.transform.position.x - _mainPoint.x) < 1f) continue;

            Item item = hit.collider.gameObject.GetComponent<Item>();

            if (item != null && (transform.position - item.transform.position).magnitude < distance)
            {
                distance = (transform.position - item.transform.position).magnitude;
                _targetItem = item;
                isSearch = true;
            }
        }
        if (!isSearch) _targetItem = null;

    }
}
