using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpannerPlayerCharacter : PlayerCharacter
{
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
