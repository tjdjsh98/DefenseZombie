using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GeneratCharacterPeriodically : MonoBehaviour
{

    [SerializeField] float _genTime;
    float _time;

    [SerializeField] List<CharacterName> _genList;
 
    void Update()
    {
        if(_time < _genTime)
        {
            _time += Time.deltaTime;
        }
        else
        {
            _time = 0;
            if (Client.Instance.ClientId == -1) {
                Manager.Character.GenerateCharacter(_genList[Random.Range(0, _genList.Count)], transform.position);
            }
            else
            {
                if(Client.Instance.IsMain) 
                {
                    Manager.Character.RequestGenerateCharacter(_genList[Random.Range(0, _genList.Count)], transform.position);
                }
            }
        }
    }
}
