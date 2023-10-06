using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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

    public void Init()
    {
        time = (_levels.Count >= 0 ? _levels[0].nextInterval : 10);
        StartCoroutine(CorStartLevel());
    }

    private void Update()
    {
      
    }

    IEnumerator CorStartLevel()
    {
        while (true)
        {
            // 싱글일 때
            if (Client.Instance.ClientId == -1)
            {
                if (!IsStartLevel)
                {
                    time -= UnityEngine.Time.fixedDeltaTime;
                    if (time <= 0)
                    {
                        if (_levels.Count <= _level) _level = 0;

                        IsStartLevel = true;
                        time = _levels[_level].nextInterval;

                        Vector3 genPosition = new Vector3(-20, -4f, 0);
                        for (int i = 0; i < _levels[_level].count; i++)
                        {
                            string enemyName = _levels[_level].enemyName;
                            Character character = Manager.Character.GenerateCharacter(enemyName, genPosition);

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
                            yield return new WaitForSeconds(_levels[_level].genInterval);
                        }
                    }
                }
            }
            // 멀티이고, 메인 클라이언트일 때
            else if (Client.Instance.IsMain)
            {
                if (time > 50)
                {
                    time = 0;

                    Vector3 genPosition = Random.Range(0, 2) == 0 ? new Vector3(20, -3.88f, -3.88f) : new Vector3(-20, -3.88f, 0);
                    for (int i = 0; i < Random.Range(1, 2); i++)
                    {
                        string enemyName = "";
                        enemyName = "Zombie";
                        Manager.Character.GenerateAndSendPacket(enemyName, genPosition);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

[System.Serializable]
public class Level
{
    public string levelName;
    public float genInterval;
    public string enemyName;
    public int count;
    public float nextInterval;
}