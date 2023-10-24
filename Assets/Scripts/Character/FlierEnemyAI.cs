using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class FlierEnemyAI : EnemyAI 
{
    [SerializeField] float _flyHeight;

    protected override void AI()
    {
        if (!Client.Instance.IsMain && Client.Instance.ClientId != -1) return;
        if (_character.Target != null && !_character.Target.gameObject.activeSelf) _character.Target = null;
        if (_character.IsAttacking) return;

        if (_character.CharacterState == CharacterState.Idle)
        {
            if (_character.Target == null)
            {
                Search();

                Vector3 moveDirection = Vector3.zero;

                if (Mathf.Abs(_character.transform.position.y - _flyHeight) >= 0.1)
                {
                    moveDirection.y = _flyHeight - transform.position.y;
                }
                _character.SetCharacterDirection(moveDirection);
            }
            else
            {
                PlayerCharacter character = Util.GetGameObjectByPhysics<PlayerCharacter>(transform.position, AttackRange, Define.PlayerLayerMask);
                Building building = Util.GetGameObjectByPhysics<Building>(transform.position, AttackRange, Define.BuildingLayerMask);


                if (character != null || building != null)
                {
                    _character.StopMove();
                    _character.SetCharacterDirection(Vector2.zero);
                    _character.IsAttacking = true;
                    _character.CharacterState = CharacterState.Attack;
                    Client.Instance.SendCharacterInfo(_character);
                }
                else
                {
                    _character.IsAttacking = false;

                    Vector3 moveDirection = Vector3.zero;

                    moveDirection.x = _character.Target.transform.position.x - transform.position.x;
                    // 1보다 멀다면 자신의 높이로
                    if(Mathf.Abs(_character.transform.position.y - _flyHeight) >= 0.1 &&Mathf.Abs(moveDirection.x) > 1)
                    {
                        moveDirection.y = _flyHeight - transform.position.y;
                    }
                    // 1보다 가깝다면 내려간다.
                    else
                    {
                        moveDirection.y = _character.Target.transform.position.y - transform.position.y;
                    }

                  
                    _character.SetCharacterDirection(moveDirection);
                }
            }
        }
        else
        {
            _character.SetCharacterDirection(Vector2.zero);
        }
    }

}
