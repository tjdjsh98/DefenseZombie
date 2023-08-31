using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Action AttackHandler;
    public Action AttackEndHandler;

    public void Attack()
    {
        AttackHandler?.Invoke();
    }
    public void AttackEnd()
    {
        AttackEndHandler?.Invoke();
    }
}
