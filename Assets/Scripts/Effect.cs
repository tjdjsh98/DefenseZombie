using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Effect : MonoBehaviour
{
    [SerializeField] EffectName _effectName;
    public EffectName EffectName => _effectName;
}
