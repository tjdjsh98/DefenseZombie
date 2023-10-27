using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UIManager : MonoBehaviour
{
    const string _canvasName = "Canvas";

    Dictionary<UIName, UIBase> _uiDictionary = new Dictionary<UIName, UIBase>();
    public void Init()
    {
        UIBase[] uis = GameObject.Find(_canvasName).GetComponentsInChildren<UIBase>();

        foreach (UIBase ui in uis)
        {
            ui.Init();
            _uiDictionary.Add(ui.UIName, ui);
        }
    }

    public UIBase GetUI(UIName name)
    {
        return _uiDictionary[name];
    }
}
