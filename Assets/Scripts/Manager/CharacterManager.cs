using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterManager : MonoBehaviour
{
    Character _main;

    public static int _singleCharacterId = 1000;
    public Character MainCharacter
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
                        Character p = null;
                        _characterDictionary.TryGetValue(Client.Instance.ClientId, out p);
                        _main = p;
                    }
                }
            }
           return _main;
        }
    }

    Dictionary<int, Character> _characterDictionary = new Dictionary<int, Character>();

    public void Init()
    {

    }
    public Character GenerateCharacter(string name,Vector3 position,bool isDummy = false)
    {
        string newName = (isDummy?"Dummy":"") + name;

        Character characterOrigin = Manager.Data.GetCharacter(newName);

        if (characterOrigin == null) return null;

        C_GenerateCharacter pkt = new C_GenerateCharacter();

        Character character = Instantiate(characterOrigin);
        character.transform.position = position;
        
        character.IsDummy = isDummy;


        character.CharacterId = ++_singleCharacterId;

        _characterDictionary.Add(character.CharacterId, character);

        return character;
    }

    public Character GetCharacter(int id)
    {
        Character character = null;

        _characterDictionary.TryGetValue(id, out character);

        return character;
    }

    public void RemoveCharacter(int id)
    {
        Character character = null;

        _characterDictionary.TryGetValue(id, out character);

        if(character != null && Client.Instance.IsMain)
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
        character.CharacterId = ++_singleCharacterId;
        character.IsDummy = isDummy;
            
        _characterDictionary.Add(_singleCharacterId, character);

        Client.Instance.SendGenreateCharacter(name, _singleCharacterId, position);

        return character;
    }

    // 패킷으로 받은 정보로 더미 캐릭터를 만듭니다.
    // 자신의 캐릭터의 ID와 맞는 캐릭터와 맞으면 조작 가능 캐릭터가 나옵니다.
    public Character GeneratePacketCharacter(S_BroadcastGenerateCharacter packet)
    {
        string newName = packet.characterName;

        bool isDummy = false;
        if (packet.characterId == Client.Instance.ClientId)
            isDummy = false;
        else
            if (Client.Instance.IsMain && packet.characterId > 100)
                isDummy = false;
            else
                isDummy = true;

        if (isDummy)
        {
            newName = "Dummy" + packet.characterName;
        }

        Character characterOrigin = Manager.Data.GetCharacter(newName);
        if (characterOrigin == null) return null;

        Character character = Instantiate(characterOrigin);

        character.CharacterId = packet.characterId;
        character.transform.position = new Vector3(packet.posX,packet.posY, packet.posZ);
      
        character.IsDummy = isDummy;
    

        _characterDictionary.Add(packet.characterId, character);

        return character;
    }

    public void GeneratePacketCharacter(S_EnterSyncInfos packet)
    {
        foreach (var player in packet.characterInfos)
        {

            bool isDummy = false;
            if (player.characterId == Client.Instance.ClientId)
            {
                isDummy = false;
            }
            else
            {
                if (Client.Instance.IsMain && player.characterId > 100)
                {
                    isDummy = false;
                }
                else
                {
                    isDummy = true;
                }
            }
            string newName = player.characterName;

            if (isDummy)
            {
                newName = "Dummy" + player.characterName;
            }
            Character characterOrigin = Manager.Data.GetCharacter(newName);
            if (characterOrigin == null) continue;

            Character character = Instantiate(characterOrigin);

            character.CharacterId = player.characterId;
            character.transform.position = new Vector3(player.posX, player.posY, player.posZ);
            character.IsDummy = isDummy;
           

            _characterDictionary.Add(player.characterId, character);
        }
    }
}
