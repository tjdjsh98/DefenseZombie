using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlierEnemyAI : EnemyAI
{
    [SerializeField] float _flyHeight;

    protected override void AI()
    {
        if (!Client.Instance.IsMain && Client.Instance.ClientId != -1) return;
        if (_target != null && !_target.gameObject.activeSelf) _target = null;
        if (_character.IsAttacking) return;

        _attackTime+= Time.deltaTime;

        if (_character.IsDamaged)
        {
            _character.SetCharacterDirection(Vector2.zero);
            return;
        }
        if (_target == null)
        {
            Search();

            Vector3 moveDirection = Vector3.zero;

            if (!CheckIsEnoughFly())
            {
                moveDirection.y = 1;
            }
            _character.SetCharacterDirection(moveDirection);
        }
        else
        {
            
            Character character = Util.GetGameObjectByPhysics<Character>(transform.position, AttackRange, Define.PlayerLayerMask);
            Building building = Util.GetGameObjectByPhysics<Building>(transform.position, AttackRange, Define.BuildingLayerMask);

            if (character != null || building != null)
            {
                // 원거리 공격하는 캐릭터면 자신의 위치까지 올라가서 공격
                if (_weapon.AttackData.projectile != null && !CheckIsEnoughFly())
                {
                    Vector3 moveDirection = Vector3.up;
                    _character.SetCharacterDirection(moveDirection);

                    return;
                }

                if (_attackTime >= _attackDelay && _weapon.IsEnableAttack)
                {
                    _attackTime = 0;
                    if (character != null)
                        _character.Turn(character.transform.position.x - transform.position.x);
                    if (building != null)
                        _character.Turn(building.transform.position.x - transform.position.x);

                    _character.StopMove();
                    _character.SetCharacterDirection(Vector2.zero);
                    _character.IsAttacking = true;
                    _character.IsEnableMoveWhileAttack = false;
                    if (character)
                        _weapon.TargetPosition = character.transform.position;
                    if (building)
                        _weapon.TargetPosition = building.transform.position;
                    Client.Instance.SendCharacterControlInfo(_character);
                }
            }
            else
            {
                Vector3 moveDirection = Vector3.zero;

                moveDirection.x = _target.transform.position.x - (transform.position.x + AttackRange.center.x);
                // 1보다 멀다면 자신의 높이로
                if (!CheckIsEnoughFly() && Mathf.Abs(moveDirection.x) > 1)
                {
                    moveDirection.y = 1;
                }
                // 1보다 가깝다면 내려간다.
                else
                {
                    moveDirection.y = _target.transform.position.y - transform.position.y;
                }


                _character.SetCharacterDirection(moveDirection);
            }
        }

    }


    bool CheckIsEnoughFly()
    {
        List<GameObject> list = Util.Raycast(transform.position, Vector3.down, _flyHeight, Define.GroundLayerMask);

        if (list.Count > 0) return false;

        return true;
    }
}
