using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class House : MonoBehaviour
{
    Character _character;

    public void EnterCharacter(Character character)
    {
        _character = character;
        _character.HideCharacter();
        Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 12;
    }

    public void LeaveCharacter()
    {
        _character.ShowCharacter();
        _character = null;
        Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 16;
    }
}
