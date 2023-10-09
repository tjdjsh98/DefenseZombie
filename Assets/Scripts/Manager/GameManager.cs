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