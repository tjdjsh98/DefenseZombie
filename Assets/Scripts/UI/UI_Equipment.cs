using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Equipment : UIBase
{
    CharacterEquipment _characterEquipment;

    class EquipmentSlot
    {
        public Image itemImage;
        public Image itemBackground;
        public TextMeshProUGUI textMesh;
    }

    Dictionary<CharacterParts, EquipmentSlot> _slots = new Dictionary<CharacterParts, EquipmentSlot>();


    public override void Init()
    {
        gameObject.SetActive(false);

        for(int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            bool success = false;
            CharacterParts part = CharacterParts.Head;

            if(transform.GetChild(0).GetChild(i).name.Equals("LegSlot"))
            {
                part = CharacterParts.Legs;
                success = true;
            }
            if (transform.GetChild(0).GetChild(i).name.Equals("HandSlot"))
            {
                part = CharacterParts.FrontHand;
                success = true;
            }
            if (transform.GetChild(0).GetChild(i).name.Equals("BodySlot"))
            {
                part = CharacterParts.Body;
                success = true;
            }
            if (transform.GetChild(0).GetChild(i).name.Equals("WeaponSlot"))
            {
                part = CharacterParts.FrontWeapon;
                success = true;
            }
            if (transform.GetChild(0).GetChild(i).name.Equals("HatSlot"))
            {
                part = CharacterParts.Hat;
                success = true;
            }

            if (success)
            {
                Image itemImage = null;
                Image backgroundImage = null;
                TextMeshProUGUI text = null;

                for (int j = 0; j < transform.GetChild(0).GetChild(i).childCount; j++)
                {
                    if (transform.GetChild(0).GetChild(i).GetChild(j).name.Equals("Back"))
                        backgroundImage = transform.GetChild(0).GetChild(i).GetChild(j).GetComponent<Image>();
                    if (transform.GetChild(0).GetChild(i).GetChild(j).name.Equals("Image"))
                        itemImage = transform.GetChild(0).GetChild(i).GetChild(j).GetComponent<Image>();
                    if (transform.GetChild(0).GetChild(i).GetChild(j).name.Equals("Text"))
                        text = transform.GetChild(0).GetChild(i).GetChild(j).GetComponent<TextMeshProUGUI>();
                }

                _slots.Add(part,new EquipmentSlot() { itemBackground= backgroundImage , itemImage = itemImage, textMesh = text});
            }
        }

        _isDone = true;
        
    }


    public void Open(CustomCharacter character)
    {
        if (gameObject.activeSelf) return;
        _characterEquipment = character.GetComponent<CharacterEquipment>();

        if (_characterEquipment == null) return;

        Manager.Input.UIMouseDownHandler += OnMouseDown;

        _characterEquipment.EquipmentChanged += Refresh;
        Refresh();

        gameObject.SetActive(true);
    }

    void Refresh()
    {

        if(_characterEquipment.BodyItemId != 0)
        {
            Item item = Manager.Item.GetItem(_characterEquipment.BodyItemId);
            _slots[CharacterParts.Body].itemImage.sprite = item.ItemData.ItemThumbnail;
            _slots[CharacterParts.Body].itemImage.rectTransform.sizeDelta = Util.CalcFitSize(80, item.ItemData.ItemThumbnail);
            _slots[CharacterParts.Body].textMesh.text = string.Empty;
        }
        else
        {
            _slots[CharacterParts.Body].itemImage.sprite = null;
            _slots[CharacterParts.Body].textMesh.text = "Body";
        }

        if (_characterEquipment.HandItemId != 0)
        {
            Item item = Manager.Item.GetItem(_characterEquipment.HandItemId);
            _slots[CharacterParts.FrontHand].itemImage.sprite = item.ItemData.ItemThumbnail;
            _slots[CharacterParts.FrontHand].itemImage.rectTransform.sizeDelta = Util.CalcFitSize(80, item.ItemData.ItemThumbnail);
            _slots[CharacterParts.FrontHand].textMesh.text = string.Empty;
        }
        else
        {
            _slots[CharacterParts.FrontHand].itemImage.sprite = null;
            _slots[CharacterParts.FrontHand].textMesh.text = "Hand";
        }

        if (_characterEquipment.HatItemId != 0)
        {
            Item item = Manager.Item.GetItem(_characterEquipment.HatItemId);
            _slots[CharacterParts.Hat].itemImage.sprite = item.ItemData.ItemThumbnail;
            _slots[CharacterParts.Hat].itemImage.rectTransform.sizeDelta = Util.CalcFitSize(80, item.ItemData.ItemThumbnail);
            _slots[CharacterParts.Hat].textMesh.text = string.Empty;
        }
        else
        {
            _slots[CharacterParts.Hat].itemImage.sprite = null;
            _slots[CharacterParts.Hat].textMesh.text = "Hat";
        }

        if (_characterEquipment.LegsItemId != 0)
        {
            Item item = Manager.Item.GetItem(_characterEquipment.LegsItemId);
            _slots[CharacterParts.Legs].itemImage.sprite = item.ItemData.ItemThumbnail;
            _slots[CharacterParts.Legs].itemImage.rectTransform.sizeDelta = Util.CalcFitSize(80, item.ItemData.ItemThumbnail);
            _slots[CharacterParts.Legs].textMesh.text = string.Empty;
        }
        else
        {
            _slots[CharacterParts.Legs].itemImage.sprite = null;
            _slots[CharacterParts.Legs].textMesh.text = "Legs";
        }

        if (_characterEquipment.EquipWeaponName != WeaponName.None)
        {
            WeaponData item = Manager.Data.GetWeaponData(_characterEquipment.EquipWeaponName);
            _slots[CharacterParts.FrontWeapon].itemImage.sprite = item.ThumbnailSprite;
            _slots[CharacterParts.FrontWeapon].itemImage.rectTransform.sizeDelta = Util.CalcFitSize(80, item.ThumbnailSprite);
            _slots[CharacterParts.FrontWeapon].textMesh.text = string.Empty;
        }
        else
        {
            _slots[CharacterParts.FrontWeapon].itemImage.sprite = null;
            _slots[CharacterParts.FrontWeapon].textMesh.text = "Weapon";
        }
    }

    public void Close() 
    {
        if (!gameObject.activeSelf) return;

        _characterEquipment.EquipmentChanged -= Refresh;

        Manager.Input.UIMouseDownHandler -= OnMouseDown;

        _characterEquipment = null;

        gameObject.SetActive(false);

    }

    void OnMouseDown(List<GameObject> list)
    {
        foreach(var part in _slots.Keys)
        {
            if(list.Contains(_slots[part].itemBackground.gameObject))
            {
                if (part == CharacterParts.FrontWeapon)
                    _characterEquipment.TakeOffWeapon();
                else
                    _characterEquipment.TakeOffOther(part);
            }
        }
    }

    public void TakeOffHead()
    {
        _characterEquipment.TakeOffOther(CharacterParts.Head);
    }
    public void TakeOffWeapon()
    {
        _characterEquipment.TakeOffOther(CharacterParts.FrontWeapon);
    }
    public void TakeOffLegs()
    {
        _characterEquipment.TakeOffOther(CharacterParts.Legs);
    }
}
