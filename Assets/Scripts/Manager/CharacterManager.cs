using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CharacterManager : MonoBehaviour
{
    Character _main;

    public static int _characterId = 100;
    public Character MainCharacter
    {
        get
        {
            if (_main == null)
            {
                // �̱� �̶�
                if (Client.Instance.ClientId == -1)
                {
                    _main = Manager.Character.GenerateCharacter(CharacterName.CustomCharacter, Vector3.zero) as Character;
                }
                else
                {
                    // ��Ƽ�� ��
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

    public Action<int,Character> ReciveGenPacket;

    public void Init()
    {
        
    }
    public Character GenerateCharacter(CharacterName name,Vector3 position)
    {
        Character characterOrigin = Manager.Data.GetCharacter(name);

        if (characterOrigin == null) return null;


        Character character = Instantiate(characterOrigin);
        character.transform.position = position;
        
        character.CharacterId = ++_characterId;

        _characterDictionary.Add(character.CharacterId, character);

        return character;
    }

    public Character GetCharacter(int id)
    {
        Character character = null;

        _characterDictionary.TryGetValue(id, out character);

        return character;
    }

    public void RequestRemoveCharacter(int id)
    {
        Client.Instance.SendRequestRemoveCharacter(id);
    }

    public void RemoveCharacter(int id)
    {
        Character character = null;

        _characterDictionary.TryGetValue(id, out character);

        if (character != null)
        {
            Destroy(character.gameObject);
        }
        _characterDictionary.Remove(id);
    }


    public Character GenerateAndSendPacket(CharacterName name, Vector3 position, bool isPlayerCharacter = false)
    {
        Character characterOrigin = Manager.Data.GetCharacter(name);

        if (characterOrigin == null) return null;

        Character character = Instantiate(characterOrigin);
        character.transform.position = position;
        if (Client.Instance.ClientId == -1)
            character.CharacterId = ++_characterId;
            
        _characterDictionary.Add(character.CharacterId, character);


        return character;
    }

    // ĳ���� ����� ���� �������� ��û�մϴ�.
    public int RequestGenerateCharacter(CharacterName name, Vector3 position, bool isPlayerCharacter = false)
    {
        int requestNumber = UnityEngine.Random.Range(3000, 99999);

        Client.Instance.SendRequestGenreateCharacter(name, position, requestNumber, isPlayerCharacter);

        return requestNumber;
    }

    // ��Ŷ���� ���� ������ ���� ĳ���͸� ����ϴ�.
    // �ڽ��� ĳ������ ID�� �´� ĳ���Ϳ� ������ ���� ���� ĳ���Ͱ� ���ɴϴ�.
    public bool GeneratePacketCharacter(S_BroadcastGenerateCharacter packet)
    {
        bool isSuccess = true;
        if (!packet.isSuccess)
            isSuccess = false;

        if (_characterDictionary.ContainsKey(packet.characterId))
            isSuccess = false;
                
        Character characterOrigin = Manager.Data.GetCharacter((CharacterName)packet.characterName);
        if (characterOrigin == null) isSuccess = false;

        Character character = null;
        if (isSuccess)
        {
            character = Instantiate(characterOrigin);

            character.CharacterId = packet.characterId;
            character.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);

            _characterDictionary.Add(packet.characterId, character);
        }

        ReciveGenPacket?.Invoke(packet.requestNumber, character);
        return isSuccess;
    }

    public void GeneratePacketCharacter(S_EnterSyncInfos packet)
    {
        foreach (var pkt in packet.characterInfos)
        {
            Character characterOrigin = Manager.Data.GetCharacter((CharacterName)pkt.characterName);
            if (characterOrigin == null) continue;

            Character character = Instantiate(characterOrigin);

            character.Hp = pkt.hp;

            character.CharacterId = pkt.characterId;
            character.transform.position = new Vector3(pkt.posX, pkt.posY, pkt.posZ);

            _characterDictionary.Add(pkt.characterId, character);
        }
    }

   
}
