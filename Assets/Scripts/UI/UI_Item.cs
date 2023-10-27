using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Item : UIBase
{
    [SerializeField]TextMeshProUGUI _textOrigin;

    List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();

    public override void Init()
    {
        Manager.Game.InventoryChanagedHandler += OnInventoryChanged;

        _isDone= true;
    }

    void OnInventoryChanged()
    {
        Dictionary<ItemData, int> inventory = Manager.Game.GetInventory();

        int index = 0;
        foreach(var key in inventory.Keys)
        {
            if(_texts.Count <= index)
            {
                TextMeshProUGUI text = Instantiate(_textOrigin,transform);
                _texts.Add(text);
            }
            _texts[index].gameObject.SetActive(true);
            _texts[index].transform.localScale = Vector3.one;
            _texts[index].transform.localPosition = new Vector3(0, -index * 60);
            _texts[index].text = $"{key.ItemName.ToString()} X {inventory[key]} ";
            index++;
        }

        for(;index < _texts.Count;index++)
        {
            _texts[index].gameObject.SetActive(false);
        }
    }
}
