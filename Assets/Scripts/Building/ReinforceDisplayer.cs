using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReinforceDisplayer : MonoBehaviour
{
    [SerializeField] SpriteRenderer _resultSpriteRenderer;
    [SerializeField] GameObject _requireItemOrigin;

    Smithy _smithy;

    List<GameObject> _slotList = new List<GameObject>();
    List<SpriteRenderer> _slotImageList = new List<SpriteRenderer>();
    List<TextMeshPro> _slotCountList = new List<TextMeshPro>();

    private void Awake()
    {
        _smithy= GetComponentInParent<Smithy>();
        _smithy.ReinforeceItemSet += OnReinforeceItemSet;
        _smithy.ItemChangedHandler += OnItemChanged;

        gameObject.SetActive(false);
    }

    void OnReinforeceItemSet()
    {
        gameObject.SetActive(true);
        OnItemChanged(false);
    }

    void OnItemChanged(bool result)
    {
        if (result)
        {
            gameObject.SetActive(false);
        }

        Item item = Manager.Item.GetItem(_smithy.ReinforceItemId);
        ItemEquipment itemEquipment = item.GetComponent<ItemEquipment>();
        int rank = itemEquipment.Rank; 

        if (item == null) return;

        _resultSpriteRenderer.sprite = item.ItemData.ItemThumbnail;
        _resultSpriteRenderer.size = Util.CalcFitSize(0.8f, _resultSpriteRenderer.sprite);
        _resultSpriteRenderer.gameObject.SetActive(true);

        for (int i = 0; i < _smithy.RequireItemList[rank].requireItems.Count; i++)
        {
            if (_slotList.Count <= i)
            {
                GameObject slot = Instantiate(_requireItemOrigin, transform);
                _slotList.Add(slot);
                _slotImageList.Add(_slotList[i].transform.Find("ItemImage").GetComponent<SpriteRenderer>());
                _slotCountList.Add(_slotList[i].transform.Find("Count").GetComponent<TextMeshPro>());
            }

            ItemData itemData = Manager.Data.GetItemData(_smithy.RequireItemList[rank].requireItems[i].itemName);
            _slotImageList[i].sprite = itemData.ItemThumbnail;
            _slotImageList[i].size = Util.CalcFitSize(0.8f, itemData.ItemThumbnail);
            _slotCountList[i].text = $"{_smithy.RequireItemList[rank].requireItems[i].currentCount}/{_smithy.RequireItemList[rank].requireItems[i].requireCount}";

            Vector3 slotLocalPos = new Vector3(-(_smithy.RequireItemList[rank].requireItems.Count - 1) / 2f * 1.2f + i * 1.2f, 0, 0);
            _slotList[i].transform.localPosition = slotLocalPos;
        }

        gameObject.SetActive(true);
    }
}
