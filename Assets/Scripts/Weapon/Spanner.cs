using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spanner : Weapon
{
    [SerializeField] float _dashPower;
    [SerializeField] float _coolDuration = 0.5f;
    [SerializeField] float _coolElapsed;
    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            _isPress = false;
        }
        if (_character.IsAttacking) return;

        if (_coolElapsed < _coolDuration)
        {
            _coolElapsed += Time.deltaTime;
            return;
        }
       

        if (Input.GetKeyDown(KeyCode.A))
        {   
            if(Mathf.Abs(_character.CurrentSpeed) >= _character.Speed || !_character.IsContactGround)
            {
                _attackType = 0;
            }
            else
            {
                _attackType = 1;
            }
            _isPress = true;
            _character.IsAttacking = true;
            _character.AttackType = _attackType;
            _animatorHandler.AttackHandler = Attack;
            _animatorHandler.AttackEndHandler = OnAttackEnd;
            if (_attackType == 0)
            {
                if (_attacks[_attackType].attackEffect != null)
                {
                    GameObject effect = Instantiate(_attacks[_attackType].attackEffect);

                    Vector3 point = _attacks[_attackType].attackEffectPoint;
                    point.x *= _character.transform.localScale.x > 0 ? 1 : -1;

                    effect.transform.position = transform.position + point;
                    Vector3 scale = Vector3.one;
                    scale.x = _character.transform.localScale.x > 0 ? 1 : -1;
                    effect.transform.localScale = scale;

                    _character.RigidBody.velocity = new Vector2(_character.RigidBody.velocity.x, 0);

                }
                _character.RigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                _character.AddForce(_character.transform.localScale.x > 0 ? Vector2.right : Vector2.left, _dashPower);
            }
            _character.CharacterState = CharacterState.Attack;
            _coolElapsed = 0;

        }
    }

    protected override void OnAttackEnd()
    {
        _character.IsAttacking = false;
        
        _character.RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _animatorHandler.AttackHandler = null;
        _animatorHandler.AttackEndHandler = null;
        if (_character.CharacterState == CharacterState.Attack)
        {
            _character.CharacterState = CharacterState.Idle;
        }
    }
}
