using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager : MonoBehaviour
{
    float startTime = 30;
    float time;
    public float Time => time;
    [SerializeField] Material _mat;
    [SerializeField] int _length;

    int _level = 0;
    [SerializeField] List<Level> _levels;

    [SerializeField] Dictionary<ItemData, int> _inventory = new Dictionary<ItemData, int>();

    public bool IsStartLevel { get; private set; }
    public int SummonCount { private set; get; }

    public UI_Commander Commander;

    List<int> _generateRequestList = new List<int>();

    public Action InventoryChanagedHandler;

    public void Init()
    {
        time = (_levels.Count >= 0 ? _levels[0].nextInterval : 10);
        Manager.Character.ReciveGenPacket += SetDiscountSummonCount;
        StartCoroutine(CorStartLevel());
    }
    IEnumerator CorStartLevel()
    {
        while (true)
        {
            // 싱글일 때, 혹은 멀티인데 메인 클라이언트라면
            if (Client.Instance.ClientId == -1 || Client.Instance.IsMain)
            {
                if (!IsStartLevel)
                {
                    time -= UnityEngine.Time.fixedDeltaTime;
                    if (time <= 0)
                    {
                        if (_levels.Count <= _level) _level = 0;

                        for(int i = 0; i < 3; i++)
                            Manager.Building.GenerateBuilding(BuildingName.Rock, new Vector2Int(UnityEngine.Random.Range(-20, 20),-3));

                        IsStartLevel = true;
                        time = _levels[_level].nextInterval;

                        Vector3 genPosition = new Vector3(20, -4f, 0);
                        for (int i = 0; i < _levels[_level].count; i++)
                        {
                            CharacterName enemyName = _levels[_level].enemyName;

                            if (Client.Instance.ClientId == -1)
                            {
                                Character character = Manager.Character.GenerateCharacter(enemyName, genPosition);

                                if (character != null) {
                                    SummonCount++;
                                    character.DeadHandler += () =>
                                    {
                                        SummonCount--;
                                        if (SummonCount <= 0)
                                        {
                                            IsStartLevel = false;
                                            _level++;
                                            SummonCount = 0;
                                        }
                                    };
                                }
                            }
                            else
                            {
                                _generateRequestList.Add(Manager.Character.RequestGenerateCharacter(enemyName, genPosition));
                            }

                            yield return new WaitForSeconds(_levels[_level].genInterval);
                        }
                    }
                }
            }
            
            yield return new WaitForFixedUpdate();
        }
    }

    void SetDiscountSummonCount(int requsetNumber,Character character)
    {
        if (_generateRequestList.Contains(requsetNumber))
        {
            _generateRequestList.Remove(requsetNumber);
            if (character != null)
            {
                SummonCount++;
                character.DeadHandler += () =>
                {
                    SummonCount--;
                    if (SummonCount == 0)
                    {
                        IsStartLevel = false;
                        _level++;
                    }

                };
            }
        }
    }

    public void AddItem(ItemData data)
    {
        if (data == null) return;

        if(_inventory.ContainsKey(data))
        {
            _inventory[data]++;
        }
        else
        {
            _inventory.Add(data, 1);
        }

        InventoryChanagedHandler?.Invoke();
    }

    public Dictionary<ItemData,int> GetInventory()
    {
        return _inventory;
    }
}

[System.Serializable]
public class Level
{
    public string levelName;
    public float genInterval;
    public CharacterName enemyName;
    public int count;
    public float nextInterval;
}