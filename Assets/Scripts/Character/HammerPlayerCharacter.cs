using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerPlayerCharacter : Character
{
    AnimatorHandler _animatorHandler;
    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _animatorHandler.DodgeEndHandler += () => { IsEnableMove = true; IsDodge = false; };
    }
    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        if(!IsDamaged && !IsAttacking)
        {
            if(_rigidBody.velocity.x != 0)
            {
                SetAnimatorBool("Walk", true);
            }
            else
            {
                SetAnimatorBool("Walk", false);
            }
        }

        if (IsDodge)
        {
            SetAnimatorBool("Dodge", true);
            _rigidBody.velocity = new Vector2(_maxSpeed * (transform.localScale.x > 0 ? 1 : -1)
                , _rigidBody.velocity.y);
        }
        if (!IsDodge)
        {
            SetAnimatorBool("Dodge", false);
        }

        SetAnimatorInteger("AttackType", AttackType);
        SetAnimatorBool("Attack", IsAttacking);

        SetAnimatorFloat("VelocityY", _rigidBody.velocity.y);

        SetAnimatorBool("ContactGround", IsContactGround);

        if(IsConncetCombo)
        {
            SetAnimatorTrigger("ConnectCombo");
            IsConncetCombo = false;
        }
    }
}
