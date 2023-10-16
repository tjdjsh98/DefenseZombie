using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class Spanner : Weapon
{
    [SerializeField] float _dashPower;
    [SerializeField] float _coolDuration = 0.5f;
    [SerializeField] float _coolElapsed;

    protected void Update()
    {
        if (_character.IsAttacking) return;

        if (_coolElapsed < _coolDuration)
        {
            _coolElapsed += Time.deltaTime;
            return;
        }
    }

    protected override void OnAttackKeyDown()
    {
        if(_coolElapsed < _coolDuration)
        {
            return;
        }
        else
        {
            _coolElapsed = 0;
        }
        //if (Mathf.Abs(_character.CurrentSpeed) >= _character.Speed || !_character.IsContactGround)
        //{
        //    _attackType = 0;
        //}
        //else
        {
            _attackType = 0;
        }
        _character.IsAttacking = true;
        _character.AttackType = _attackType;

        if (_attackType == 0)
        {
            GameObject attackEffect = Manager.Data.GetEffect(_attacks[_attackType].attackEffectName);
            if (attackEffect != null)
            {
                GameObject effect = Instantiate(attackEffect);
                effect.transform.parent = transform;

                Vector3 point = _attacks[_attackType].attackEffectPoint;

                effect.transform.localPosition = point;
                effect.transform.localScale = Vector3.one;
            }

            _character.RigidBody.velocity = new Vector2(_character.RigidBody.velocity.x, 0);
            _character.AddForce(_character.transform.localScale.x > 0 ? Vector2.right : Vector2.left, _dashPower,
                (int)(RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation));

            _character.CharacterState = CharacterState.Attack;

            Client.Instance.SendCharacterInfo(_character);

            _coolElapsed = 0;
        }
        else if (_attackType == 1)
        {
            _character.CharacterState = CharacterState.Attack;
            Client.Instance.SendCharacterInfo(_character);
        }
    }
    protected override void OnAttackKeyUp()
    {
    }

    protected override void OnAttackEnd()
    {
        if (_attackType == 0)
        {
            _character.RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        if (_character.CharacterState == CharacterState.Attack)
        {
            _character.CharacterState = CharacterState.Idle;
        }
        Client.Instance.SendCharacterInfo(_character);
    }
}
