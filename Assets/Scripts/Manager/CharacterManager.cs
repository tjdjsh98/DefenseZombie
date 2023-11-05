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
                    Manager.Character.GenerateCharacter(CharacterName.CustomCharacter, Vector3.zero, ref _main);
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

    // �ش� ���̵� �ش��ϴ� ĳ���� ��ȯ
    public Character GetCharacter(int id)
    {
        Character character = null;

        if(_characterDictionary.TryGetValue(id, out character))
        {
            return character;
        }

        return null;
    }

    // �̱�, ��Ƽ ȥ������ ����ϴ� ĳ���� ���� �Լ�
    public int GenerateCharacter(CharacterName name, Vector3 position,ref Character character, bool isPlayerCharacter = false)
    {
        int requestNumber = -1;
        if(Client.Instance.ClientId == -1)
        {
            character = GenerateCharacter(name, position);
        }
        else
        {
            requestNumber = RequestGenerateCharacter(name, position,isPlayerCharacter);
        }

        return requestNumber;
    }
    
    // �̱� �� �� ĳ���� ����
    private Character GenerateCharacter(CharacterName name,Vector3 position)
    {
        Character characterOrigin = Manager.Data.GetCharacter(name);

        if (characterOrigin == null) return null;


        Character character = Instantiate(characterOrigin);
        character.transform.position = position;
        
        character.CharacterId = ++_characterId;

        _characterDictionary.Add(character.CharacterId, character);

        return character;
    }
    // ��Ƽ �� �� ĳ���� ����� ���� �������� ��û�մϴ�.
    private int RequestGenerateCharacter(CharacterName name, Vector3 position, bool isPlayerCharacter = false)
    {
        int requestNumber = UnityEngine.Random.Range(3000, 99999);

        Client.Instance.SendRequestGenreateCharacter(name, position, requestNumber, isPlayerCharacter);

        return requestNumber;
    }

    // �̱�, ��Ƽ ȥ������ ����ϴ� ĳ���� ���� �Լ�
    public void RemoveCharacter(int id)
    {
        if(Client.Instance.ClientId == -1)
        {
            SingleRemoveCharacter(id);
        }
        else
        {
            RequestRemoveCharacter(id);
        }
    }

    // ��Ƽ �� �� ĳ���� ������ �������� ��û�մϴ�.
    private void RequestRemoveCharacter(int id)
    {
        Client.Instance.SendRequestRemoveCharacter(id);
    }

    // �̱� �� �� ĳ���� ����
    void SingleRemoveCharacter(int id)
    {
        Character character = null;

        _characterDictionary.TryGetValue(id, out character);

        if (character != null)
        {
            Destroy(character.gameObject);
        }
        _characterDictionary.Remove(id);
    }


    // ��Ŷ���� ���� ������ ���� ĳ���͸� ����ϴ�.
    // �ڽ��� ĳ������ ID�� �´� ĳ���Ϳ� ������ ���� ���� ĳ���Ͱ� ���ɴϴ�.
    public bool GenerateCharacterByPacket(S_BroadcastGenerateCharacter packet)
    {
        if (!packet.isSuccess) return false;

        if (_characterDictionary.ContainsKey(packet.characterId))
        {
            Destroy(_characterDictionary[packet.characterId].gameObject);
            _characterDictionary.Remove(packet.characterId);
        }
                
        Character characterOrigin = Manager.Data.GetCharacter((CharacterName)packet.characterName);
        if (characterOrigin == null) return false;

        UI_Debug ui = Manager.UI.GetUI(UIName.Debug) as UI_Debug;
        ui.AddText(packet.characterId.ToString());

        Character character = null;
       
        character = Instantiate(characterOrigin);
            
        character.CharacterId = packet.characterId;
        character.transform.position = new Vector3(packet.posX, packet.posY, 0);

        _characterDictionary.Add(packet.characterId, character);

        ReciveGenPacket?.Invoke(packet.requestNumber, character);

        return true;
    }

    public void GeneratePacketCharacter(S_EnterSyncInfos packet)
    {
        foreach (var pkt in packet.characterInfos)
        {
            if (_characterDictionary.ContainsKey(pkt.characterId))
            {
                Destroy(_characterDictionary[pkt.characterId].gameObject);
                _characterDictionary.Remove(pkt.characterId);
            }
            Character characterOrigin = Manager.Data.GetCharacter((CharacterName)pkt.characterName);
            if (characterOrigin == null) continue;

            Character character = Instantiate(characterOrigin);
            character.CharacterId = pkt.characterId;

            character.DeserializeData(pkt.data);

            _characterDictionary.Add(pkt.characterId, character);
        }
    }

   
}
