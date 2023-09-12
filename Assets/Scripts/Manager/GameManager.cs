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
   
    public void Init()
    {
    }

    private void Update()
    {
        if (Client.Instance.ClientId != 1) return;

        time += Time.deltaTime;
        if (time > 20)
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
}
