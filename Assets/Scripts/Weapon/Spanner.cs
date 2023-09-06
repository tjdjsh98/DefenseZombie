using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spanner : Weapon
{
    [SerializeField] float _dashPower;
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _isPress = true;
            _character.IsAttacking = true;
            _character.AttackType = _attackType;
            _animatorHandler.AttackHandler = Attack;
            _animatorHandler.AttackEndHandler = OnAttackEnd;
            if (_attacks[_attackType].attackEffect != null)
            {
                GameObject effect = Instantiate(_attacks[_attackType].attackEffect);

                Vector3 point = _attacks[_attackType].attackEffectPoint;
                point.x *= _character.transform.localScale.x > 0 ? 1 : -1;

                effect.transform.position = transform.position + point;
                Vector3 scale = Vector3.one;
                scale.x = _character.transform.localScale.x > 0 ? 1 : -1;
                effect.transform.localScale = scale;

            }
            _character.CharacterState = CharacterState.Attack;
            _character.AddForce(_character.transform.localScale.x > 0 ? Vector2.right : Vector2.left, _dashPower);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _isPress = false;
        }
    }

    protected override void OnAttackEnd()
    {
        if (!_isPress)
        {
            _character.IsAttacking = false;
            _animatorHandler.AttackHandler = null;
            _animatorHandler.AttackEndHandler = null;
            _animatorHandler.ConnectComboHandler = null;
            if (_character.CharacterState == CharacterState.Attack)
            {
                _character.CharacterState = CharacterState.Idle;
            }
        }

    }
}
