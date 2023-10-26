using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterAndStand : InteractableObject
{
    Character _enterCharacter;
    [SerializeField]GameObject _standingPos;

    public override bool Interact(Character character)
    {

        _enterCharacter = character;
        character.transform.parent = _standingPos.transform;

        character.transform.localPosition = Vector3.zero;
        character.FreezeRigidbody();
        character.IsEnableMove = false;
        character.StopMove();

        return true;
    }

    public override bool ExitInteract(Character character)
    {
        if (_enterCharacter == null) return false;

        character.ReleaseRigidbody();
        _enterCharacter.transform.parent = transform.root;
        _enterCharacter.transform.position = transform.position;
        _enterCharacter.IsEnableMove =true;
        return true;
    }
}
