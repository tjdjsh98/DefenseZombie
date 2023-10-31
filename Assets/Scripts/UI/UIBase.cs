using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField] UIName _uiName;
    public UIName UIName=> _uiName;

    public bool _isDone = false;
    public abstract void Init();
}
