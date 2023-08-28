using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    Character _character;

    public void EnterCharacter(Character character)
    {
        _character = character;
        _character.gameObject.SetActive(false);
    }

    public void LeaveCharacter()
    {
        _character.gameObject.SetActive(true);
        _character = null;
    }
}
