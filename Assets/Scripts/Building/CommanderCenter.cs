using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CommanderCenter : InteractableObject
{
    Character _character;
    UI_Commander _ui;

    public override bool Interact(Character character)
    {
        if (_character != null) return false;
        _character = character;
        _character.IsEnableAttack = false;
        _character.HideCharacter();
        Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 12;

        if (_ui == null)
            _ui = Manager.UI.GetUI(Define.UIName.Commander) as UI_Commander;

        _ui.Open(this);

        
        return true;
    }


    public override bool ExitInteract(Character character)
    {
        if (_character == null) return false;

        _character.IsEnableAttack = true;
        _character.ShowCharacter();
        _character = null;
        Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 16;

        _ui.Close();

        return true;

    }
}
