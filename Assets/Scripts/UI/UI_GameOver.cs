using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_GameOver : UIBase
{
    bool IsDone = false;

    [SerializeField] TextMeshProUGUI _text;
    public override void Init()
    {
        gameObject.SetActive(false);
        IsDone = true;
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        _text.text = "GameOver";
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        Time.timeScale = 0.0f;
        _text.text = "Clear!";
        gameObject.SetActive(true);
    }
}
