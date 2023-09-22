using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    PlayerCharacter _main;

    public static int _characterId = 1000;
    public PlayerCharacter MainCharacter
    {
        get 
        {
            if (_main == null)
            {
                // 싱글 이때
                if (Client.Instance.ClientId == -1)
                {
                    _main = Manager.Character.GenerateCharacter("HammerCharacter", Vector3.zero) as PlayerCharacter;
                }
                else
                {
                    // 멀티일 떄
                    {
                        PlayerCharacter p = _playerList.Find((c) => { return c.CharacterId == Client.Instance.ClientId; });
                        _main = p;
                    }
                }
            }
           return _main;
        }
    }

    List<PlayerCharacter> _playerList = new List<PlayerCharacter>();
    public List<PlayerCharacter> PlayerList => _playerList;

    List<EnemyCharacter> _enemyList = new List<EnemyCharacter>();
    public List<EnemyCharacter> EnemyList => _enemyList;


    public void Init()
    {

    }
    public Character GenerateCharacter(string name,Vector3 position,bool isDummy = false)
    {
        string newName = (isDummy?"Dummy":"") + name;

        Character characterOrigin = Manager.Data.GetCharacter(newName);

        if (characterOrigin == null) return null;

        C_RequestGenerateCharacter pkt = new C_RequestGenerateCharacter();

        Character character = Instantiate(characterOrigin);
        character.transform.position = position;
        
        character.IsDummy = isDummy;
     
        if (character is PlayerCharacter)
        {
            _playerList.Add(character as PlayerCharacter);
        }
   
        if (character is EnemyCharacter)
        {
            _enemyList.Add(character as EnemyCharacter);
            character.CharacterId = ++_characterId;
        }
        return character;
    }

    public Character GetCharacter(int id)
    {
        foreach (var dummy in _playerList)
        {
            if (dummy.CharacterId == id)
            {
                return dummy;
            }
        }
        foreach (var dummy in _enemyList)
        {
            if (dummy.CharacterId == id)
            {
                return dummy;
            }
        }

        return null;
    }

    public void RemoveCharacter(int id)
    {
        int removeId = -1;
        foreach(var dummy in _playerList)
        {
            if(dummy.CharacterId == id)
            {
                Destroy(dummy.gameObject);
                _playerList.Remove(dummy);
                removeId = id;
                break;
            }
        }
        if (removeId == -1)
        {
            foreach (var dummy in _enemyList)
            {
                if (dummy.CharacterId == id)
                {
                    Destroy(dummy.gameObject);
                    _enemyList.Remove(dummy);
                    removeId = id;
                    break;
                }
            }
        }
        if(removeId != -1 && Client.Instance.IsMain)
        {
            Client.Instance.SendRemoveCharacter(id);
        }
    }
    public Character GenerateAndSendPacket(string name, Vector3 position, bool isDummy = false)
    {
        string newName = (isDummy ? "Dummy" : "") + name;

        Character characterOrigin = Manager.Data.GetCharacter(newName);

        if (characterOrigin == null) return null;

        Character character = Instantiate(characterOrigin);
        character.transform.position = position;
        character.CharacterId = ++_characterId;
        character.IsDummy = isDummy;
            
        if (character is PlayerCharacter)
        {
            _playerList.Add(character as PlayerCharacter);
        }
        if(character is EnemyCharacter)
        {
            _enemyList.Add(character as EnemyCharacter);
        }

        Client.Instance.SendGenreateCharacter(name, _characterId, position);

        return character;
    }

    public Character GenerateDummyCharacter(S_BroadcastGenerateCharacter packet)
    {
        string newName = "Dummy" + packet.characterName;

        Character characterOrigin = Manager.Data.GetCharacter(newName);
        if (characterOrigin == null) return null;

        Character character = Instantiate(characterOrigin);

        character.CharacterId = packet.characterId;
        character.transform.position = new Vector3(packet.posX,packet.posY, packet.posZ);
        character.IsDummy = true;

        if (character is PlayerCharacter)
        {
            _playerList.Add(character as PlayerCharacter);
        }
        if (character is EnemyCharacter)
        {
            _enemyList.Add(character as EnemyCharacter);
        }

        return character;
    }


}
