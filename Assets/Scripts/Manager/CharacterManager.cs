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
                // 싱글 이때
                if (Client.Instance.ClientId == -1)
                {
                    Manager.Character.GenerateCharacter(CharacterName.CustomCharacter, Vector3.zero, ref _main);
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

    Dictionary<int , Action<Character>> _requestGenerateAction = new Dictionary<int, Action<Character>>();
    Dictionary<int , Action> _requsetRemoveAction = new Dictionary<int , Action>();

    public void Init()
    {
        
    }

    // 해당 아이디에 해당하는 캐릭터 반환
    public Character GetCharacter(int id)
    {
        Character character = null;

        if(_characterDictionary.TryGetValue(id, out character))
        {
            return character;
        }

        return null;
    }

    // 싱글, 멀티 혼용으로 사용하는 캐릭터 생성 함수
    public int GenerateCharacter(CharacterName name, Vector3 position,ref Character character, bool isPlayerCharacter = false,Action<Character> addAction = null)
    {
        int requestNumber = 0;
        if(Client.Instance.ClientId == -1)
        {
            character = GenerateCharacter(name, position);
        }
        else
        {
            requestNumber = RequestGenerateCharacter(name, position,isPlayerCharacter,addAction);
        }

        return requestNumber;
    }
    
    // 싱글 일 떄 캐릭터 생성
    private Character GenerateCharacter(CharacterName name,Vector3 position)
    {
        Character characterOrigin = Manager.Data.GetCharacter(name);

        if (characterOrigin == null) return null;


        Character character = Instantiate(characterOrigin);
        character.transform.position = position;
        character.CharacterId = ++_characterId;

        character.Init();

        _characterDictionary.Add(character.CharacterId, character);

        return character;
    }
    // 멀티 일 떄 캐릭터 만드는 것을 서버에게 요청합니다.
    private int RequestGenerateCharacter(CharacterName name, Vector3 position, bool isPlayerCharacter = false, Action<Character> addAction = null)
    {
        int requestNumber = UnityEngine.Random.Range(3000, 99999);

        if (addAction != null)
        {
            _requestGenerateAction.Add(requestNumber, addAction);
        }


        Client.Instance.SendRequestGenreateCharacter(name, position, requestNumber, isPlayerCharacter);

        return requestNumber;
    }

    // 싱글, 멀티 혼용으로 사용하는 캐릭터 삭제 함수
    public int RemoveCharacter(int id,Action removeAction = null)
    {
        int requesetNumber = 0;
        if (Client.Instance.ClientId == -1)
        {
            SingleRemoveCharacter(id);
        }
        else
        {
            requesetNumber = RequestRemoveCharacter(id,removeAction);
        }

        return requesetNumber;
    }

    // 멀티 일 떄 캐릭터 삭제를 서버에게 요청합니다.
    private int RequestRemoveCharacter(int id, Action removeAction = null)
    {
        int requesetNumber = UnityEngine.Random.Range(3000, 99999);

        if (removeAction != null)
        {
            _requsetRemoveAction.Add(requesetNumber, removeAction);
        }

        Client.Instance.SendRequestRemoveCharacter(id);

        return requesetNumber;
    }

    // 싱글 일 떄 캐릭터 삭제
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


    // 패킷으로 받은 정보로 더미 캐릭터를 만듭니다.
    // 자신의 캐릭터의 ID와 맞는 캐릭터와 맞으면 조작 가능 캐릭터가 나옵니다.
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

        character.Init();

        _characterDictionary.Add(packet.characterId, character);

        if (_requestGenerateAction.ContainsKey(packet.requestNumber))
        {
            _requestGenerateAction[packet.requestNumber]?.Invoke(character);
            _requestGenerateAction.Remove(packet.requestNumber);
        }


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
            character.transform.position= new Vector3(pkt.posX,pkt.posY, 0);
            character.CharacterId = pkt.characterId;

            character.Init();

            character.DeserializeData(pkt.data1);
            character.DeserializeControlData(pkt.data2);

            _characterDictionary.Add(pkt.characterId, character);
        }
    }


    public void RemoveCharacterByPacket(S_BroadcastRemoveCharacter pkt)
    {
        Character character = null;

        _characterDictionary.TryGetValue(pkt.characterId, out character);

        if (character != null)
        {
            Destroy(character.gameObject);
        }
        _characterDictionary.Remove(pkt.characterId);

        if (_requsetRemoveAction.ContainsKey(pkt.requestNumber))
        {
            _requsetRemoveAction[pkt.requestNumber]?.Invoke();
            _requsetRemoveAction.Remove(pkt.requestNumber);
        }
    }
}
