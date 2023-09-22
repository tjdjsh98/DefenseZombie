using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    Character _character;

    LineRenderer _backLine;
    LineRenderer _frontLine;

    private void Awake()
    {
        _character = GetComponentInParent<Character>();
        _backLine = transform.Find("Back").GetComponent<LineRenderer>();
        _frontLine = transform.Find("Front").GetComponent<LineRenderer>();

        _character.TurnedHandler += OnTurned;
    }

    void Update()
    {
        if(_character == null) return;
        _frontLine.SetPosition(1, new Vector3(((float)_character.Hp / _character.MaxHp) -0.5f, 0, 0));
    }

    void OnTurned(float direction)
    {
        if (direction == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x)* direction > 0 ? 1 : -1;

        transform.localScale = scale;
    }
}
