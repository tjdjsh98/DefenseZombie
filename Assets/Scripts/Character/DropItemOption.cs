using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DropItemOption : MonoBehaviour, ICharacterOption
{
    Character _character;

    [SerializeField] List<ItemName> data;
    public void Init()
    {
        _character= GetComponent<Character>();

        _character.DeadHandler += OnDead;
    }

    void OnDead()
    {
        Manager.Item.GenerateItem(data.GetRandom(),transform.position);
        _character.DeadHandler -= OnDead;
    }
}
