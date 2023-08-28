using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    float time;
    public void Init()
    {

    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > 10)
        {
            time = 0;

            Manager.Character.GenerateCharacter("Zombie", Random.Range(0,2) == 0 ?  new Vector3(10, 0, 0) : new Vector3(-10,0,0));
        }
    }
}
