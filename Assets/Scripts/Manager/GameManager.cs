using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
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

    public Action InventoryChanagedHandler;

    int _rockCount;
    int _rockMaxCount = 1;
    int _treeCount;
    int _treeMaxCount = 1;

    public Building MainCore;

    public void Init()
    {

        if (Client.Instance.IsSingle || (!Client.Instance.IsSingle && Client.Instance.IsMain))
        {
            Building building = null;
            for (int x = -30; x <= 30; x++)
            {
                Manager.Building.GenerateBuilding(BuildingName.GrassTile, new Vector2Int(x, -2), ref building);
                for (int y = -3; y >= -10; y--)
                {
                    Manager.Building.GenerateBuilding(BuildingName.GroundTile, new Vector2Int(x, y), ref building);
                }
            }
            Manager.Building.GenerateBuilding(BuildingName.Core, new Vector2Int(0, -1), ref building);
        }
        time = (_levels.Count >= 0 ? _levels[0].nextInterval : 10);

        if(Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            StartCoroutine(CorStartLevel());
        }
    }

    private void Update()
    {
        if (MainCore == null)
        {
            MainCore = Manager.Building.GetBuildingByName(BuildingName.Core);
        }
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
                    if ((int)time != (int)(time - UnityEngine.Time.fixedDeltaTime))
                    {
                        time -= UnityEngine.Time.fixedDeltaTime;
                        Client.Instance.SendManagerInfo(ManagerName.Game, SerializeData());
                    }
                    else
                    {
                        time -= UnityEngine.Time.fixedDeltaTime;
                    }
                    if (time <= 0)
                    {
                        if (_levels.Count <= _level) _level = 0;

                        IsStartLevel = true;
                        index = 0;
                        time = _levels[_level].nextInterval;
                        Client.Instance.SendManagerInfo(ManagerName.Game, SerializeData());
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
                            int requsetNumber = Manager.Character.GenerateCharacter(enemyName, genPos, ref character,false, (c) =>
                            {
                                SummonCount++;
                                c.DeadHandler += () =>
                                {
                                    SummonCount--;
                                    Client.Instance.SendManagerInfo(ManagerName.Game, SerializeData());
                                };
                                CustomCharacter customCharacter = c as CustomCharacter;
                                if(levelCharacter.hp != 0)
                                    customCharacter.SetHp(levelCharacter.hp);
                                if (levelCharacter.setupData != null)
                                {
                                    customCharacter.SetSetup(levelCharacter.setupData);
                                }
                            });

                            if (character != null)
                            {
                                SummonCount++;
                                character.DeadHandler += () =>
                                {
                                    SummonCount--;
                                };
                                CustomCharacter customCharacter = character as CustomCharacter;

                                if (levelCharacter.setupData != null)
                                {
                                    customCharacter.SetSetup(levelCharacter.setupData);
                                }
                            }
                        }
                        Client.Instance.SendManagerInfo(ManagerName.Game, SerializeData());

                        yield return new WaitForSeconds(_levels[_level].levelCharacterList[index].nextGenDelay);

                        index++;
                    }
                    else
                    {
                        if(SummonCount <= 0)
                        {
                            IsStartLevel = false;
                            _level++;
                            Client.Instance.SendManagerInfo(ManagerName.Game, SerializeData());
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

    public string SerializeData()
    {
        Util.StartWriteSerializedData();

        Util.WriteSerializedData(IsStartLevel);
        Util.WriteSerializedData(time);
        Util.WriteSerializedData(SummonCount);


        return Util.EndWriteSerializeData();
    }

    public void DeserializeData(string stringData)
    {
        if (stringData == null) return;
        Util.StartReadSerializedData(stringData);

        IsStartLevel = Util.ReadSerializedDataToBoolean();
        time = Util.ReadSerializedDataToFloat();
        SummonCount = Util.ReadSerializedDataToInt();
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
    public SetupData setupData;
    public int spawnCount;
    public bool isRandomSpawn;
    public bool isRightSpawn;
    public float nextGenDelay;
    public int hp;
}