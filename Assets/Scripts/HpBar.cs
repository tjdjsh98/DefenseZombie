using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    IHp _hp;

    LineRenderer _backLine;
    LineRenderer _frontLine;

    private void Awake()
    {
        _hp = GetComponentInParent<IHp>();
        _backLine = transform.Find("Back").GetComponent<LineRenderer>();
        _frontLine = transform.Find("Front").GetComponent<LineRenderer>();

    }

    void Update()
    {
        if(_hp == null) return;

        _frontLine.SetPosition(1, new Vector3(((float)_hp.Hp / _hp.MaxHp) -0.5f, 0, 0));
        transform.localScale = transform.parent.localScale;
    }
}
