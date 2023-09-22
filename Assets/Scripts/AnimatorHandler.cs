using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    Character _character;

    public Action AttackStartedHandler;
    public Action AttackHandler;
    public Action AttackEndHandler;
    public Action ConnectComboHandler;
    public Action DestroyHandler;
    public Action DodgeEndHandler;
    

    private void Awake()
    {
        _character = GetComponent<Character>();
    }
    public void AttackStarted()
    {
        AttackStartedHandler?.Invoke();
    }
    public void Attack()
    {
        AttackHandler?.Invoke();
    }
    public void AttackEnd()
    {
        _character.IsAttacking = false;
        AttackEndHandler?.Invoke();
    }

    public void ConnectCombo()
    {
        ConnectComboHandler?.Invoke();
    }

    public void Destroy()
    {
        DestroyHandler?.Invoke();
        Destroy(gameObject);
    }

    public void DodgeEnd()
    {
        DodgeEndHandler?.Invoke();
    }

    public void SetAttackType(int type)
    {
        _character.AttackType = type;
    }
}
