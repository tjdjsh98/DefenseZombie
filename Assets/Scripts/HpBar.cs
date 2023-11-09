using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    IHp _hp;

    LineRenderer _backLine;
    LineRenderer _frontLine;
    TextMeshPro _text;

    int _preHp;

    private void Awake()
    {
        _hp = GetComponentInParent<IHp>();
        _backLine = transform.Find("Back").GetComponent<LineRenderer>();
        _frontLine = transform.Find("Front").GetComponent<LineRenderer>();
        _text = transform.Find("Text").GetComponent<TextMeshPro>();


        if (transform.parent.gameObject.tag == "Building" || transform.parent.gameObject.tag == "Player")
        {
            _frontLine.startColor = Color.blue;
            _frontLine.endColor = Color.blue;
            _backLine.startColor = new Color(0,8/255f,96/255f);
            _backLine.endColor= new Color(0,8/255f,96/255f);
        }


    }

    void Update()
    {
        transform.localScale = transform.parent.localScale;

        if(_hp == null) return;

        if (_preHp != _hp.Hp)
        {
            _frontLine.SetPosition(1, new Vector3(((float)_hp.Hp / _hp.MaxHp) - 0.5f, 0, 0));
            _text.text = $"{_hp.Hp}/{_hp.MaxHp}"; 
            _preHp = _hp.Hp;
        }
    }
}
