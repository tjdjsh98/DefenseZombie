using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerPlayerCharacter : PlayerCharacter
{
    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        if(CharacterState == CharacterState.Idle)
        {
            if(_currentSpeed != 0)
            {
                SetAnimatorBool("Walk", true);
            }
            else
            {
                SetAnimatorBool("Walk", false);
            }
        }

        SetAnimatorBool("Attack", IsAttacking);
    }
}
