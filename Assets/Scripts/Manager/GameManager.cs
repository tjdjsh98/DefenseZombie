using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

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

    int _rockCount;
    int _rockMaxCount = 1;
    int _treeCount;
    int _treeMaxCount = 1;

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
                // 웨이브 별 몬스터 생성
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

                        Vector3 genPosition = new Vector3(20, -1f, 0);
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

                // 일정 갯수의 나무와 바위 지속 생성

                if(_rockMaxCount > _rockCount)
                {
                    Building building = Manager.Data.GetBuilding(BuildingName.Rock);
                    Vector2Int cellPos = Manager.Building.FindRandomEmptyGroundInRange(building.BuildingSize);

                    if(cellPos.x != -999)
                    {
                        Building rock = Manager.Building.GenerateBuilding(BuildingName.Rock,cellPos);

                        if(rock != null)
                        {
                            _rockCount++;
                            rock.DestroyedHandler += ()=> { _rockCount--; } ;
                        }
                    }
                }

                if(_treeMaxCount > _treeCount)
                {
                    Building building = Manager.Data.GetBuilding(BuildingName.Tree);
                    Vector2Int cellPos = Manager.Building.FindRandomEmptyGroundInRange(building.BuildingSize);

                    if (cellPos.x != -999)
                    {
                        Building tree = Manager.Building.GenerateBuilding(BuildingName.Tree, cellPos);

                        if (tree != null)
                        {
                            _treeCount++;
                            tree.DestroyedHandler += () => { _treeCount--; };
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

    public void GenerateSupplies()
    {
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-10f, 10f);
        pos.y = 10;
        Manager.Item.GenerateItem(ItemName.Supplies, pos);
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