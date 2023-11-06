using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PistalPlayerCharacter : Character
{
    [SerializeField] float _walkSpeed;
    [SerializeField] float _backWalkSpeed;

    SpriteRenderer _upperSpriteRenderer;
    SpriteRenderer _lowerSpriteRenderer;

    public override void Init()
    {
        base.Init();

        _upperSpriteRenderer = transform.Find("Model").Find("Upper").GetComponent<SpriteRenderer>();
        _lowerSpriteRenderer = transform.Find("Model").Find("Lower").GetComponent<SpriteRenderer>();
    }

    protected override void ControlAnimation()
    {
        base.ControlAnimation();

        #region 이동
        //if (CharacterState == CharacterState.Idle )
        //{
        //    bool walk, backWalk = false;

        //    if(_rigidBody.velocity.x == 0)
        //    {
        //        walk = false;
        //        backWalk = false;
        //    }
        //    else if(IsAttacking)
        //    {
        //        if((_rigidBody.velocity.x > 0 && transform.localScale.x > 0) ||
        //            (_rigidBody.velocity.x < 0 && transform.localScale.x < 0))
        //        {
        //            walk = true;
        //            backWalk = false;
        //        }
        //        else
        //        {
        //            walk = false;
        //            backWalk = true;
        //        }
        //    }
        //    else
        //    {
        //        walk = true;
        //        backWalk = false;
        //    }
        //    _maxSpeed = backWalk ? _backWalkSpeed : _walkSpeed;

        //    SetAnimatorBool("Walk", walk);
        //    SetAnimatorBool("BackWalk", backWalk);
        //}
        #endregion

        #region 공격
        SetAnimatorBool("Attack",IsAttacking);
        #endregion

    }

    public override void HideCharacter()
    {
        _rigidBody.isKinematic = true;
        _capsuleCollider.enabled = false;
        _upperSpriteRenderer.enabled = false;
        _lowerSpriteRenderer.enabled = false;
    }

    public override void ShowCharacter()
    {
        _rigidBody.isKinematic = false;
        _capsuleCollider.enabled = true;
        _upperSpriteRenderer.enabled = true;
        _lowerSpriteRenderer.enabled = true;
    }

    public override void Turn(float direction)
    {
        if (!IsAttacking)
        {
            if (TurnedHandler != null)
            TurnedHandler(direction);
        
            Vector3 scale = Vector3.one;
            scale.x = direction > 0 ? 1 : -1;
            transform.localScale = scale;
        }
    }
}