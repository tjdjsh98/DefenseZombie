using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectManager : MonoBehaviour
{
    public void Init()
    {

    }


    // 싱글, 멀티 이펙트 생성
    public int GenerateEffect(EffectName effectName, Vector3 pos, ref GameObject effect)
    {
        int requestNumber = 0;
        if(Client.Instance.IsSingle)
        {
            effect = GenerateEffect(effectName, pos);
        }
        else
        {
            requestNumber = RequestGenerateEffect(effectName, pos);
        }

        return requestNumber;
    }

    // 싱글일 때
    private GameObject GenerateEffect(EffectName effectName, Vector3 pos)
    {
        GameObject effectOrigin = Manager.Data.GetEffect(effectName);

        if (effectOrigin == null) return null;

        GameObject effect = Instantiate(effectOrigin);
        effect.transform.position = pos;

        return effect;
    }

    // 멀티 일 때 생성

    private int RequestGenerateEffect(EffectName effectName, Vector3 pos)
    {
        int requestNumber = Random.Range(100, 1000);

        Client.Instance.SendRequestGenreateEffect(effectName,pos,requestNumber);

        return requestNumber;
    }

    public void GenerateEffectByPacket(S_BroadcastGenerateEffect packet)
    {
        int requestNumber = packet.requestNumber;

        Vector3 pos = new Vector3(packet.posX, packet.posY);

        GenerateEffect((EffectName)packet.effectName, pos);
    }
}
