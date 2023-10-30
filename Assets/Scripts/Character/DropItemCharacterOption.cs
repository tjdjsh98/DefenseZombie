using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DropItemCharacterOption : MonoBehaviour, ICharacterOption
{
    Character _character;

    [SerializeField] List<ItemName> data;
    public void Init()
    {
        _character= GetComponent<Character>();

        if(_character != null)
            _character.DeadHandler += OnDead;
    }

    void OnDead()
    {
        Manager.Item.GenerateItem(data.GetRandom(),transform.position);
        _character.DeadHandler -= OnDead;
    }
}
