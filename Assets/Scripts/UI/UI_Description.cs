using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Description : UIBase
{
    [SerializeField] GameObject _description;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] TextMeshProUGUI _descriptionDetailText;

    StringBuilder _stringBuilder = new StringBuilder();
    public override void Init()
    {
        _description.gameObject.SetActive(false);

        _isDone = true;
    }

    public void OpenItemDescription(Item item,Vector3 position)
    {
        if (item == null) return;
        _description.gameObject.SetActive(true);

        _stringBuilder.Clear();

        if (item.ItemData.ItemType == Define.ItemType.Equipment)
        {
            _titleText.text = item.ItemData.ItemName.ToString();
            _image.sprite = item.ItemData.ItemThumbnail;
            _image.rectTransform.sizeDelta = Util.CalcFitSize(80, _image.sprite);

            ItemEquipment itemEquipment = null;
            // 무기 아이템일 경우
            if(item.ItemData.EquipmentData== null)
            {
                itemEquipment = item.GetComponent<ItemEquipment>();

                WeaponName name = WeaponName.None;
                Enum.TryParse(item.ItemData.ItemName.ToString(), true, out name);
                WeaponData weaponData = Manager.Data.GetWeaponData(name);

                if(weaponData!= null) 
                {
                    _stringBuilder.AppendLine($"강화횟수\t{itemEquipment.Rank}");
                    if(itemEquipment.AddedAttack == 0)
                        _stringBuilder.AppendLine($"공격력\t{weaponData.AttackList[0].damage}");
                    else
                        _stringBuilder.AppendLine($"공격력\t<color=#5EFF23>{weaponData.AttackList[0].damage + itemEquipment.AddedAttack}</color> +({itemEquipment.AddedAttack})");
                    _stringBuilder.AppendLine($"재공격시간\t{weaponData.AttackList[0].attackDelay}");
                    if (weaponData.AttackList[0].projectile!= null)
                    {
                        _stringBuilder.AppendLine($"수급탄창\t{weaponData.AttackList[0].projectile.name}");
                        _stringBuilder.AppendLine($"최대탄창수\t{item.GetComponent<ItemAmmo>().MaxAmmon}");
                    }
                    _stringBuilder.AppendLine($"관통력\t{weaponData.AttackList[0].penetrationPower}");
                    _stringBuilder.AppendLine($"밀치기\t{weaponData.AttackList[0].power}");
                    _stringBuilder.AppendLine($"경직\t{weaponData.AttackList[0].stagger}");
                }
            }
            // 방어구 일 경우
            else if ((itemEquipment = item.GetComponent<ItemEquipment>()) != null)
            {
                _stringBuilder.AppendLine($"강화횟수\t{itemEquipment.Rank}");
                if(itemEquipment.AddedHp == 0)
                    _stringBuilder.AppendLine($"체력\t{itemEquipment.Hp}");
                else
                    _stringBuilder.AppendLine($"체력\t<color=#5EFF23>{itemEquipment.Hp}</color> +({itemEquipment.AddedHp})");

                if(itemEquipment.AddedDefense == 0)
                    _stringBuilder.AppendLine($"방어력\t{itemEquipment.Defense}");
                else
                    _stringBuilder.AppendLine($"방어력\t<color=#5EFF23>{itemEquipment.Defense}</color> +({itemEquipment.AddedDefense})");

                if(itemEquipment.AddedSpeed == 0)
                    _stringBuilder.AppendLine($"속도\t{itemEquipment.Speed}");
                else
                    _stringBuilder.AppendLine($"속도\t<color=#5EFF23>{itemEquipment.Speed}</color> +({itemEquipment.AddedSpeed})");
            }
        }
        _descriptionText.text = _stringBuilder.ToString();
        _descriptionDetailText.text = "";
    }

    public void OpenBuildingDescription(Building building)
    {
        if (building == null) return;

        _description.gameObject.SetActive(true);
        _stringBuilder.Clear();

        _titleText.text = building.BuildingName.ToString();
        _image.sprite = building.GetSpriteRendererList()[0].sprite;
        _image.rectTransform.sizeDelta = Util.CalcFitSize(80, _image.sprite);

        _stringBuilder.AppendLine($"체력\t{building.MaxHp}");

        _descriptionText.text = _stringBuilder.ToString();

        _descriptionDetailText.text = building.Description;
    }

    public void Close()
    {
        _description.gameObject.SetActive(false);
    }
}
