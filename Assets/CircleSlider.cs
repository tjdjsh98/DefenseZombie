using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSlider : MonoBehaviour
{
    SpriteRenderer _outline;
    SpriteRenderer _circle;

    private void Awake()
    {
        _outline = transform.Find("CircleOutline").GetComponent<SpriteRenderer>();
        _circle = transform.Find("GreenCircle").GetComponent<SpriteRenderer>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void SetRatio(float ratio)
    {
        _outline.material.SetFloat("_Slider", ratio);
        _circle.material.SetFloat("_Slider", ratio);
    }
}
