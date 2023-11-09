using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    Character _character;
    SpriteRenderer _arrowSpriteRenderer;
    TextMeshPro _text;

    private void Awake()
    {
        _character = GetComponentInParent<Character>();
        _arrowSpriteRenderer = transform.Find("Arrow").GetComponent<SpriteRenderer>();
        _text = transform.Find("Text").GetComponent<TextMeshPro>();

        _arrowSpriteRenderer.enabled = false;
        _text.enabled = false;
    }

    public void Update()
    {
        if (_character == null) return;

        transform.localScale = _character.transform.localScale;

        if (_arrowSpriteRenderer.enabled == false)
        {
            if ((_character.CharacterId > 0 && _character.CharacterId < 10))
            {
                _arrowSpriteRenderer.enabled = true;
                _text.enabled = true;

                if (Client.Instance.IsSingle || Client.Instance.ClientId == _character.CharacterId)
                {
                    _arrowSpriteRenderer.color = Color.red;
                }
                else
                {
                    _arrowSpriteRenderer.color = Color.green;
                }
                _text.text = _character.CharacterId.ToString();
            }
        }

    }
}
