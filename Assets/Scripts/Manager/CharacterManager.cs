using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public void Init()
    {

    }

    public Character GenerateCharacter(string name,Vector3 position)
    {
        Character characterOrigin = Manager.Data.GetCharacter(name);

        if (characterOrigin == null) return null;

        Character character = Instantiate(characterOrigin );
        character.transform.position = position;

        return character;
    }
}
