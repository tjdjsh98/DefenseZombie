using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Smithy : UIBase
{
    Smithy _smity;
    CustomCharacter _customCharacter;
    CharacterEquipment _characterEquipment;

    [SerializeField] Image _weaponBackground;
    [SerializeField] Image _weaponiamge;
    [SerializeField] Image _hatBackground;
    [SerializeField] Image _hatIamge;

    int _weaponId = 0;
    int _hatId = 0;

    public override void Init()
    {
        gameObject.SetActive(false);
    }

    public void Open(Smithy smithy, CustomCharacter customCharacter)
    {
        if (smithy == null || smithy.gameObject == null) return;

        _smity = smithy;
        _customCharacter = customCharacter;
        _characterEquipment= customCharacter.GetComponent<CharacterEquipment>();

        Item weaponItem = Manager.Item.GetItem(_characterEquipment.WeaponId);
        if (weaponItem != null)
        {
            _weaponId = weaponItem.ItemId;
            _weaponiamge.sprite = weaponItem.ItemData.ItemThumbnail;
            _weaponiamge.rectTransform.sizeDelta = Util.CalcFitSize(80, weaponItem.ItemData.ItemThumbnail);
        }
        else
        {
            _weaponId = 0;
            _weaponiamge.sprite = null;
            _weaponiamge.rectTransform.sizeDelta = Vector2.zero;
        }
        Item hatItem = Manager.Item.GetItem(_characterEquipment.HatItemId);
        if (hatItem != null)
        {
            _hatId = hatItem.ItemId;
            _hatIamge.sprite = hatItem.ItemData.ItemThumbnail;
            _hatIamge.rectTransform.sizeDelta = Util.CalcFitSize(80, weaponItem.ItemData.ItemThumbnail);
        }
        else
        {
            _hatId = 0;
            _hatIamge.sprite = null;
            _hatIamge.rectTransform.sizeDelta = Vector2.zero;
        }

        Manager.Input.UIMouseDownHandler += OnUIMouseDown;

        gameObject.SetActive(true);
    }

    public void Close()
    {
        Manager.Input.UIMouseDownHandler -= OnUIMouseDown;

        gameObject.SetActive(false);
    }

    void OnUIMouseDown(List<GameObject> list)
    {
        foreach(var gameObject in list)
        {
            if(gameObject == _hatBackground.gameObject && _hatId != 0)
            {
                _smity.SetEnforeceItem(_hatId);
                Close();
                return;
            }
            if (gameObject == _weaponBackground.gameObject && _weaponId != 0)
            {
                _smity.SetEnforeceItem(_weaponId);
                Close();
                return;
            }
        }
    }
}
