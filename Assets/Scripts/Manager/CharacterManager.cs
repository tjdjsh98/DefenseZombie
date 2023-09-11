using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    PlayerCharacter _main;
    public PlayerCharacter MainCharacter
    {
        get 
        { 
            if(_main == null)
            {
                PlayerCharacter p = _playerList.Find((c) => { return c.CharacterId == Client.Instance.ClientId; });
                _main = p;
            }
           return _main;
        }
    }

    List<PlayerCharacter> _playerList = new List<PlayerCharacter>();
    public List<PlayerCharacter> PlayerList => _playerList;

    List<DummyCharacter> _dummyList = new List<DummyCharacter>();
    public List<DummyCharacter> DummyList => _dummyList;
    public void Init()
    {

    }

    public Character GenerateCharacter(string name,Vector3 position)
    {
        Character characterOrigin = Manager.Data.GetCharacter(name);

        if (characterOrigin == null) return null;

        Character character = Instantiate(characterOrigin);
        character.transform.position = position;

        if(character is PlayerCharacter)
            _playerList.Add(character as PlayerCharacter);
        if(character is DummyCharacter)
            _dummyList.Add (character as DummyCharacter);

        return character;
    }
}
