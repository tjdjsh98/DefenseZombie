using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TextDisplayer : UIBase
{
    [SerializeField] TextMeshPro _textOrigin;
    List<TextMeshPro> _textList = new List<TextMeshPro>();

    public override void Init()
    {
        _isDone = true;
    }

    public void DisplayText(string text,Vector3 position, Color color, float fontSize)
    {
        TextMeshPro _text = null;

        foreach(var t in _textList)
        {
            if (!t.gameObject.activeSelf)
            {
                _text = t;
                break;
            }
        }
        
        if (_text == null)
            _text = GenerateNewText();

        _text.text = text;
        _text.color = color;
        _text.fontSize = fontSize;
        _text.transform.position = position;
        _text.gameObject.SetActive(true);

        StartCoroutine(CorMoveTextUp(_text));
    }

    TextMeshPro GenerateNewText()
    {
        TextMeshPro _text = Instantiate(_textOrigin);
        _textList.Add(_text);
        return _text;
    }


    IEnumerator CorMoveTextUp(TextMeshPro text)
    {
        float time = 0;
        while(time < 2)
        {
            time += Time.deltaTime;
            text.transform.position += Vector3.up * Time.fixedDeltaTime;
            Color color = text.color;
            color.a = (2-time) / 2f;
            text.color = color; 

            yield return new WaitForFixedUpdate();
        }

        text.gameObject.SetActive(false);
    }
    
}
