using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEnemyAI : EnemyAI
{
    CustomCharacter _customCharacter;
    public override void Init()
    {
        base.Init();
        _customCharacter = _character as CustomCharacter;
    }

    protected override void AI()
    {
        if (!Client.Instance.IsMain && Client.Instance.ClientId != -1) return;

        if (_target != null && !_target.gameObject.activeSelf) _target = null;
        if (_character.IsAttacking) return;

        if (_character.CharacterState == CharacterState.Idle)
        {
            _attackTime += Time.deltaTime;
            if (_target == null)
            {
                Search();
            }
            else
            {

                Character character = Util.GetGameObjectByPhysics<Character>(transform.position, AttackRange, Define.PlayerLayerMask);
                Building building = Util.GetGameObjectByPhysics<Building>(transform.position, AttackRange, Define.BuildingLayerMask);

                // ÆÈ ¿òÁ÷ÀÓ
                if(character != null && _weapon.WeaponAttackData.projectile != null)
                {
                    Vector3 targetPos = character.transform.position + character.CharacterSize.center;
                    _customCharacter.RotationFrontHand(targetPos);
                }

                if (character != null || building != null)
                {
                    _character.StopMove();
                    _character.SetCharacterDirection(Vector2.zero);

                    if (_attackDelay < _attackTime)
                    {
                        _attackTime = 0;
                        _character.IsAttacking = true;
                        _character.CharacterState = CharacterState.Attack;
                        Client.Instance.SendCharacterInfo(_character);
                    }

                    return;
                }
                _character.SetCharacterDirection(_target.transform.position - transform.position);
                _character.Turn((_target.transform.position - transform.position).x);
            }
        }
        else
        {
            _character.SetCharacterDirection(Vector2.zero);
        }
    }
}
