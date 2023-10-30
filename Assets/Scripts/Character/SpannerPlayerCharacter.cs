using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpannerPlayerCharacter : Character
{
    AnimatorHandler _animatorHandler;
    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<AnimatorHandler>();
        _animatorHandler.DodgeEndHandler += () => { CharacterState = CharacterState.Idle; };
    }
    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        if (CharacterState == CharacterState.Idle)
        {
            if (_currentSpeed != 0)
            {
                SetAnimatorBool("Walk", true);
            }
            else
            {
                SetAnimatorBool("Walk", false);
            }
        }   

        if(CharacterState == CharacterState.Dodge && !IsDodge) 
        {
            SetAnimatorBool("Dodge", true);
            IsDodge = true;
        }
        if(IsDodge)
        {
            _rigidBody.velocity = new Vector2(_maxSpeed * (transform.localScale.x > 0? 1 : -1)
                , _rigidBody.velocity.y);
        }
        if (CharacterState != CharacterState.Dodge && IsDodge)
        {
            SetAnimatorBool("Dodge", false);
            IsDodge = false;
        }

        SetAnimatorInteger("AttackType", AttackType);
        SetAnimatorBool("Attack", IsAttacking);

        SetAnimatorFloat("VelocityY", _rigidBody.velocity.y);


        SetAnimatorBool("ContactGround", IsContactGround);

        if (IsConncetCombo)
        {
            SetAnimatorTrigger("ConnectCombo");
            IsConncetCombo = false;
        }
    }
}
