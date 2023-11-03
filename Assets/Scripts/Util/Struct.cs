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
}

// ĳ���Ͱ� �������� ���� �� �ִ� ������Ʈ
public interface IEnableInsertItem
{
    // ĳ���Ͱ� �������� �ְų� ���ų� �ϴ� ������ �ϸ� ����
    public Action<bool> ItemChangedHandler { get; set; }  
    public bool InsertItem(ItemName itemName);
    public bool CheckIsFinish();
}

public interface IDataSerializable
{
    public string SerializeData();
 

    public void DeserializeData(string stringData);

}
