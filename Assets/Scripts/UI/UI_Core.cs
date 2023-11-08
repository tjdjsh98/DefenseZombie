using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Core : UIBase
{
    [SerializeField] Image _hpImage;
    [SerializeField] TextMeshProUGUI _text;

    IHp _coreHp;
    int _preHp;

    float _initHpWidth;

    bool _initDone;
    public override void Init()
    {
        _initHpWidth = _hpImage.rectTransform.sizeDelta.x;

        _initDone = true;
    }

    private void Update()
    {
        if (!_initDone) return;

        if(Manager.Game.MainCore != null)
        {
            if (_coreHp == null)
                _coreHp = Manager.Game.MainCore.GetComponent<IHp>();

            if (_preHp != _coreHp.Hp)
            {
                Vector2 size = _hpImage.rectTransform.sizeDelta;
                size.x = (float)_initHpWidth * _coreHp.Hp / _coreHp.MaxHp;
                _hpImage.rectTransform.sizeDelta = size;
                _text.text = $"{_coreHp.Hp}/{_coreHp.MaxHp}";
                _preHp = _coreHp.Hp;
            }
        }
    }
}
