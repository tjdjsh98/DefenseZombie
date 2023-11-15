using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ammo : UIBase
{
    ItemAmmo _itemAmmo;

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image _image;

    public override void Init()
    {
        _isDone = true;

        gameObject.SetActive(false);
    }

    public void Open(ItemAmmo itemAmmo)
    {
        _itemAmmo = itemAmmo;
        _itemAmmo.AmmoChangedHandler += Refresh;
        Refresh();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        if (!gameObject.activeSelf) return;
        _itemAmmo.AmmoChangedHandler -= Refresh;
        gameObject.SetActive(false);

    }

    void Refresh()
    {
        if (_itemAmmo == null) return;

        _image.sprite = _itemAmmo.RequireItem.ItemThumbnail;
        _image.rectTransform.sizeDelta = Util.CalcFitSize(100, _image.sprite);
        _text.text = $"{_itemAmmo.currentAmmo}/{_itemAmmo.MaxAmmon}";
    }
}
