using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using static Define;

public class CustomEnemyAI : EnemyAI
{
    CustomCharacter _customCharacter;
    public override void Init()
    {
        base.Init();
        _customCharacter = _character as CustomCharacter;
        _animatorHandler.AttackEndHandler += ()=>
        {
            _customCharacter.RotationHandZero();
        };
    }

    protected override void AI()
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

            // ÆÈ ¿òÁ÷ÀÓ
            if (character != null && _weapon.WeaponAttackData.projectile != null && _weapon.WeaponAttackData.projectile.ProjectileType == ProjectileType.Linear)
            {
                Vector3 targetPos = character.transform.position + character.CharacterSize.center;
                _customCharacter.RotationHand(targetPos);
            }

            if (character != null || building != null)
            {
                _character.StopMove();
                _character.SetCharacterDirection(Vector2.zero);

                if (_attackDelay < _attackTime)
                {
                    Vector3 targetPos = Vector3.zero;
                    if (character != null)
                        targetPos = character.transform.position + character.CharacterSize.center;
                    else if (building)
                        targetPos = building.transform.position;
                    _attackTime = 0;
                    bool success = false;
                    if (_weapon.WeaponAttackData.projectile != null)
                    {
                        if (_weapon.WeaponAttackData.projectile.ProjectileType == ProjectileType.Arrow)
                        {
                            Arrow arrow = _weapon.WeaponAttackData.projectile as Arrow;

                            for(int i = 90; i >= 0; i--)
                            {
                                Vector3 direction = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad) * transform.localScale.x, Mathf.Sin(i * Mathf.Deg2Rad)).normalized;

                                Vector3 startPos = Vector3.zero;
                                if (_weapon.WeaponData.IsFrontWeapon)
                                    startPos = _customCharacter.GetFrontHandPosition();
                                else
                                    startPos = _customCharacter.GetBehindHandPosition();

                                success = arrow.PredictTrajectory(startPos, direction, targetPos);
                                if (success)
                                {
                                    _customCharacter.RotationHand(startPos + direction);
                                    break;
                                }
                                if (success) break;

                            }
                        }
                    }
                    else
                    {
                        success = true;
                    }
                    if (success)
                    {
                        _character.IsAttacking = true;
                        Client.Instance.SendCharacterControlInfo(_character);
                    }
                }

                return;
            }
            _character.SetCharacterDirection(_target.transform.position - transform.position);
            _character.Turn((_target.transform.position - transform.position).x);
        }
    }
}
