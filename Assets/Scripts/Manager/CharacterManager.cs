using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    Character _main;
    public Character MainCharacter
    {
        get { if(_main == null)
                _main = GameObject.FindWithTag("Player").GetComponent<Character>();
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

        Character character = Instantiate(characterOrigin);
        character.transform.position = position;

        return character;
    }
}
