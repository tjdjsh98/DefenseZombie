using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hammer : Weapon
{
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _attackType = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _attackType = 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _character.CharacterState = CharacterState.Attack;
            _character.AttackType = _attackType;
            _isPress = true;
            _character.IsAttacking = true;
            _animatorHandler.AttackHandler = Attack;
            _animatorHandler.AttackEndHandler = OnAttackEnd;

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
            if (_character.CharacterState == CharacterState.Attack)
            {
                _character.CharacterState = CharacterState.Idle;
            }
        }

    }
}
