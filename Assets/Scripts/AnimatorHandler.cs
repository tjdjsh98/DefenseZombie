using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Action AttackHandler;
    public Action AttackEndHandler;
    public Action ConnectComboHandler;
    public Action DestroyHandler;
    public void Attack()
    {
        AttackHandler?.Invoke();
    }
    public void AttackEnd()
    {
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
}
