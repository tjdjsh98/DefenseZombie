using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hammer : Weapon
{
    bool _connectCombo;

    public override void Init()
    {
        base.Init();
    }
    protected override void OnAttackKeyDown()
    {
        if (!_character.IsAttacking)
        {
            _attackType = 1;
            _character.IsEnableMoveWhileAttack= false;
            _character.AttackType = _attackType;
            _character.IsAttacking = true;
            _connectCombo = false;
            _animatorHandler.ConnectComboHandler = OnConnectCombo;
        }
        else
        {
            if (!_connectCombo)
            {
                _connectCombo = true;
            }
        }
    }
    protected override void OnAttackKeyUp()
    {
    }

    protected void OnConnectCombo()
    {
        if (_connectCombo)
        {
            _character.IsConncetCombo = true;
            _attackType = 2;
            _connectCombo = false;
        }
    }

    protected override void OnAttackEnd()
    {
        _animatorHandler.ConnectComboHandler = null;
        
    }
}
