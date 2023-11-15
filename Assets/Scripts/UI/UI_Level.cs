using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Level : UIBase
{
    [SerializeField] TextMeshProUGUI _timeText;

    public override void Init()
    {
        _isDone = true;
    }
    private void Update()
    {
        if(!_isDone) return;

        if(!Manager.Game.IsStartLevel)
        {
            _timeText.text = $"{(int)(Manager.Game.Time/60) }:{(int)Manager.Game.Time}";
        }
        else
        {
            _timeText.text = $"{Manager.Game.Level.ToString()} Round";
        }
    }
}
