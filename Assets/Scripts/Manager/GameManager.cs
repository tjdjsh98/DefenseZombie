using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameManager : MonoBehaviour
{
    float startTime = 30;
    float time;
    public float Time => time;
    [SerializeField] Material _mat;
    [SerializeField] int _length;

    int _level = 0;
    [SerializeField] List<Level> _levels;

    public bool IsStartLevel { get; private set; }
    public int SummonCount { private set; get; }

    public UI_Commander Commander;

    List<int> _generateRequestList = new List<int>();

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