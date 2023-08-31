using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    PlayerCharacter _main;
    public PlayerCharacter MainCharacter
    {
        get { if(_main == null)
                _main = GameObject.Find("Character").GetComponent<PlayerCharacter>();
                    return _main; 
        }
    }
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
