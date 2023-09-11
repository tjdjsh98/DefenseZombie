using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameManager : MonoBehaviour
{
    float time;
    [SerializeField] Material _mat;
    [SerializeField] int _length;

    Character _character;
    Character PlayerCharacter
    {
        get
        {
            if (_character == null)
            {
                if (GameObject.Find("Character"))
                {
                    _character = GameObject.Find("Character").GetComponent<Character>();
                }
            }
            return _character;
        }
    }
    public void Init()
    {
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 712038)
        {
            time = 0;

            Vector3 genPosition = Random.Range(0, 2) == 0 ? new Vector3(20, -3.88f, -3.88f) : new Vector3(-20, -3.88f, 0);
            for (int i = 0; i < Random.Range(3, 7); i++)
            {
                string enemyName = "";

                switch(Random.Range(0,3))
                {
                    case 0:
                        enemyName = "Horriy";
                        break;
                    case 1:
                        enemyName = "ShieldJr";
                        break;
                    case 2:
                        enemyName = "Zombie";
                        break;
                    default:
                        enemyName = "SheidJr";
                        break;
                }
                Manager.Character.GenerateCharacter(enemyName, genPosition);
            }
        }
    }
        
}
