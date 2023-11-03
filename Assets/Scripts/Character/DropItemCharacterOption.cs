using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DropItemCharacterOption : MonoBehaviour, ICharacterOption
{
    Character _character;

    [SerializeField] List<ItemName> data;
    public bool IsDone { get; set; }

    public void DataDeserialize()
    {
    }

    public void DataSerialize()
    {
    }

    public void Init()
    {
        _character= GetComponent<Character>();

        if(_character != null)
            _character.DeadHandler += OnDead;

        IsDone = true;
    }

    void OnDead()
    {
        Item item = null;
        Manager.Item.GenerateItem(data.GetRandom(),transform.position,ref item);
        _character.DeadHandler -= OnDead;
    }
}
