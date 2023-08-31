using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    Character _character;

    public void EnterCharacter(Character character)
    {
        _character = character;
        _character.HideCharacter();
    }

    public void LeaveCharacter()
    {
        _character.ShowCharacter();
        _character = null;
    }
}
