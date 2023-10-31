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
        GameObject canvas = GameObject.Find(_canvasName);

        for(int i = 0; i < canvas.transform.childCount; i++)
        {
            UIBase ui = canvas.transform.GetChild(i).GetComponent<UIBase>();
            if(ui != null)
            {
                ui.Init();
                _uiDictionary.Add(ui.UIName, ui);
            }
        }
    }

    public UIBase GetUI(UIName name)
    {
        return _uiDictionary[name];
    }
}
