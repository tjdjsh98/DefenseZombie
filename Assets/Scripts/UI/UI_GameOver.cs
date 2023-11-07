using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameOver : UIBase
{
    public override void Init()
    {
        gameObject.SetActive(false);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameObject.SetActive(true);
    }
}
