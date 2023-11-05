using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Debug : UIBase
{
    [SerializeField]TextMeshProUGUI _text;
    public override void Init()
    {
        
    }

    public void AddText(string text)
    {
        _text.text += text + "\n";
    }
    
}
