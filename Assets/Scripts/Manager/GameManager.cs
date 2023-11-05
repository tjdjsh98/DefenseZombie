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
        if (Client.Instance.IsSingle || (!Client.Instance.IsSingle && Client.Instance.IsMain))
        {
            Building building = null;
            for (int x = -5; x <= 5; x++)
            {
                Manager.Building.GenerateBuilding(BuildingName.GrassTile, new Vector2Int(x, -2), ref building);
                for (int y = -3; y >= -5; y--)
                {
                    Manager.Building.GenerateBuilding(BuildingName.GroundTile, new Vector2Int(x, y), ref building);
                }
            }
        }


        time = (_levels.Count >= 0 ? _levels[0].nextInterval : 10);
        Manager.Character.ReciveGenPacket += SetDiscountSummonCount;
        StartCoroutine(CorStartLevel());
    }
    IEnumerator CorStartLevel()
    {
        int index = 0;
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

                        IsStartLevel = true;
                        index = 0;
                        time = _levels[_level].nextInterval;
                    }
                }
                else
                {
                    if (index < _levels[_level].levelCharacterList.Count)
                    {
                        LevelCharacter levelCharacter = _levels[_level].levelCharacterList[index];
                        CharacterName enemyName = levelCharacter.characterName;

                        for (int i = 0; i < (levelCharacter.spawnCount == 0 ? 1 : levelCharacter.spawnCount); i++)
                        {
                            Vector3 genPos = Vector3.zero;
                            if (levelCharacter.isRandomSpawn)
                                genPos = new Vector3(Random.Range(0, 2) == 0 ? 20 : -20, -2f, 0);
                            else if (levelCharacter.isRightSpawn)
                                genPos = new Vector3(20, -2f, 0);
                            else if (!levelCharacter.isRandomSpawn)
                                genPos = new Vector3(-20, -2f, 0);

                            Character character = null;
                            int requsetNumber = Manager.Character.GenerateCharacter(enemyName, genPos, ref character);

                            if (requsetNumber != -1)
                            {
                                _generateRequestList.Add(requsetNumber);
                            }
                            if (character != null)
                            {
                                SummonCount++;
                                character.DeadHandler += () =>
                                {
                                    SummonCount--;
                                };
                            }

                            if(character != null)
                            {
                                CustomCharacter customCharacter = character as CustomCharacter;

                                if(levelCharacter.itemCharacterHas != null)
                                {
                                    Item item = null;
                                    Manager.Item.GenerateItem(levelCharacter.itemCharacterHas.ItemName,character.transform.position,ref item);
                                    customCharacter.GrapItem(item);
                                }
                            }
                        }

                        yield return new WaitForSeconds(_levels[_level].levelCharacterList[index].nextGenDelay);

                        index++;
                    }
                    else
                    {
                        if(SummonCount <= 0)
                        {
                            IsStartLevel = false;
                            _level++;
                        }
                    }
                }    

                // 일정 갯수의 나무와 바위 지속 생성

                //if(_rockMaxCount > _rockCount)
                //{
                //    Building building = Manager.Data.GetBuilding(BuildingName.Rock);
                //    Vector2Int cellPos = Manager.Building.FindRandomEmptyGroundInRange(building.BuildingSize);

                //    if(cellPos.x != -999)
                //    {
                //        Building rock = null;
                //        int requestNumber = Manager.Building.GenerateBuilding(BuildingName.Rock,cellPos, ref rock);

                //        if(rock != null)
                //        {
                //            _rockCount++;
                //            rock.DestroyedHandler += ()=> { _rockCount--; } ;
                //        }
                //    }
                //}

                //if(_treeMaxCount > _treeCount)
                //{
                //    Building building = Manager.Data.GetBuilding(BuildingName.Tree);
                //    Vector2Int cellPos = Manager.Building.FindRandomEmptyGroundInRange(building.BuildingSize);

                //    if (cellPos.x != -999)
                //    {
                //        Building tree = null;
                //        int requestNumber = Manager.Building.GenerateBuilding(BuildingName.Tree, cellPos, ref tree);

                //        if (tree != null)
                //        {
                //            _treeCount++;
                //            tree.DestroyedHandler += () => { _treeCount--; };
                //        }
                //    }
                //}


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
        Item item = null;
        Manager.Item.GenerateItem(ItemName.Supplies, pos, ref item);
    }
}

[System.Serializable]
public class Level
{
    public string levelName;
    public List<LevelCharacter> levelCharacterList;
    public float nextInterval;
}

[System.Serializable]
public class LevelCharacter
{
    public CharacterName characterName;
    public ItemData itemCharacterHas;
    public int spawnCount;
    public bool isRandomSpawn;
    public bool isRightSpawn;
    public float nextGenDelay;
}