using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour, ICharacterOption
{
    Character _character;

    public Action AttackStartedHandler;
    public Action AttackHandler;
    public Action AttackEndHandler;
    public Action ConnectComboHandler;
    public Action DestroyHandler;
    public Action DodgeEndHandler;


    public void Init()
    {
        _character = GetComponent<Character>();
    }
    public void AnimatorAttackStarted()
    {
        AttackStartedHandler?.Invoke();
    }
    public void AnimatorAttack()
    {
        AttackHandler?.Invoke();
    }
    public void AnimatorAttackEnd()
    {
        _character.IsAttacking = false;
        AttackEndHandler?.Invoke();
    }

    public void AnimatorConnectCombo()
    {
        ConnectComboHandler?.Invoke();
    }

    public void AnimatorDestroy()
    {
        DestroyHandler?.Invoke();
        Destroy(gameObject);
    }

    public void AnimatorDodgeEnd()
    {
        DodgeEndHandler?.Invoke();
    }

    public void AnimatorSetAttackType(int type)
    {
        _character.AttackType = type;
    }
}
