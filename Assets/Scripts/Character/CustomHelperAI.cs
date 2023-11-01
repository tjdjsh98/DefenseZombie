using System.Collections;
using System.Collections.Generic;
using System.Data;
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
                range = _weapon.WeaponAttackData.attackRange;

            range.center.x = transform.localScale.x > 0 ? range.center.x : -range.center.x;

            return range;

        }   
    }

    public override void Init()
    {
        base.Init();
        _weapon = GetComponent<Weapon>();
        _customCharacter= _character as CustomCharacter;
    }

    protected override void AI()
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
                _customCharacter.RotationFrontHand(targetPos);

                Range range = AttackRange;
                range.center /= 2;
                if (Util.GetIsInRange(_character.gameObject, _target.gameObject, range))
                {
                    _character.SetCharacterDirection(Vector3.zero);
                    AttackHanlder?.Invoke();
                    Client.Instance.SendCharacterInfo(_character);
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
}
