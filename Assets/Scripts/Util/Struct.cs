using System;
using UnityEngine;
using static Define;

[System.Serializable]
public struct Range
{
    public Vector3 center;
    public Vector3 size;
}


[System.Serializable]
public struct AttackData
{
    public EffectName attackEffectName;
    public Vector3 attackEffectPoint;
    public Projectile projectile;
    public Vector3 firePos;
    public EffectName hitEffectName;
    public Range attackRange;
    public Vector3 AttackDirection;
    public AttacKShape attacKShape;
    public int damage;
    public float power;
    public float stagger;
    public int penetrationPower;
    public EffectName actualAttackEffectName;
    public bool isTargeting;
    public float attackDelay;

}

// 캐릭터가 아이템을 넣을 수 있는 오브젝트
public interface IEnableInsertItem
{
    // 캐릭터가 아이템을 넣거나 빼거나 하는 행위를 하면 실행
    public Action<bool> ItemChangedHandler { get; set; }  
    public bool InsertItem(Item item);
    public bool CheckIsFinish();
}

public interface IDataSerializable
{
    public string SerializeData();
    public void DeserializeData(string stringData);

}


public interface ICharacterOption
{
    public bool IsDone { set; get; }
    public void Init();

    public void Remove();
    public void DataSerialize();

    public void DataDeserialize();
}

public interface IBuildingOption
{
    public void Init();
    public void SerializeData();

    public void DeserializeData();

    public void SerializeControlData();
    public void DeserializeControlData();
}

public interface IItemOption
{
    public bool IsDone { set; get; }
    public void Init();

    public void SerializeData();

    public void DeserializeData();

    public void SerializeControlData();
    public void DeserializeControlData();
}

