using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Level : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timeText;
    private void Update()
    {
        if(!Manager.Game.IsStartLevel)
        {
            _timeText.text = $"{(int)(Manager.Game.Time/60) }:{(int)Manager.Game.Time}";
        }
        else
        {
            _timeText.text = Manager.Game.SummonCount.ToString();
        }
    }
}
