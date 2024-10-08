using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ItemBlueprintDisplayer : MonoBehaviour
{
    [SerializeField] SpriteRenderer _resultSpriteRenderer;
    [SerializeField] GameObject _requireItemOrigin;

    Shop _shop;

    List<GameObject> _slotList = new List<GameObject>();
    List<SpriteRenderer> _slotImageList = new List<SpriteRenderer>();
    List<TextMeshPro> _slotCountList = new List<TextMeshPro>();

    private void Awake()
    {
        _shop = GetComponentInParent<Shop>();
        _shop.ItemChangedHandler += OnItemChanaged;
        _shop.MainBlueprintSetHandler += Open;
        _resultSpriteRenderer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Open()
    {
        ItemBlueprintData blueprint = _shop.MainBlueprint;

        _resultSpriteRenderer.sprite = Manager.Data.GetItemData(blueprint.ResultItemName).ItemThumbnail;
        _resultSpriteRenderer.size = Util.CalcFitSize(0.8f, _resultSpriteRenderer.sprite);
        _resultSpriteRenderer.gameObject.SetActive(true);

        for (int i = 0; i < blueprint.BlueprintItemList.Count; i++)
        {
            if (_slotList.Count <= i)
            {
                GameObject slot = Instantiate(_requireItemOrigin, transform);
                _slotList.Add(slot);
                _slotImageList.Add(_slotList[i].transform.Find("ItemImage").GetComponent<SpriteRenderer>());
                _slotCountList.Add(_slotList[i].transform.Find("Count").GetComponent<TextMeshPro>());
            }

            ItemData itemData = Manager.Data.GetItemData(blueprint.BlueprintItemList[i].name);
            _slotImageList[i].sprite = itemData.ItemThumbnail;
            _slotImageList[i].size = Util.CalcFitSize(0.8f, itemData.ItemThumbnail);
            _slotCountList[i].text = $"{blueprint.BlueprintItemList[i].currentCount}/{blueprint.BlueprintItemList[i].requireCount}";

            Vector3 slotLocalPos = new Vector3(-(blueprint.BlueprintItemList.Count - 1) / 2f * 1.2f + i * 1.2f, 0, 0);
            _slotList[i].transform.localPosition = slotLocalPos;
        }

        gameObject.SetActive(true);
    }

    void OnItemChanaged(bool isFinish)
    {
        if (!Client.Instance.IsSingle && !Client.Instance.IsMain)
        {
            if (_shop.MainBlueprint == null)
            {
                Hide();
                return;
            }
        }

        if (isFinish)
        {
            Hide();
            return;
        }

        ItemBlueprintData blueprint = _shop.MainBlueprint;
        if (blueprint != null)
        {
            for (int i = 0; i < blueprint.BlueprintItemList.Count; i++)
            {
                ItemData itemData = Manager.Data.GetItemData(blueprint.BlueprintItemList[i].name);
                _slotImageList[i].sprite = itemData.ItemThumbnail;
                _slotCountList[i].text = $"{blueprint.BlueprintItemList[i].currentCount}/{blueprint.BlueprintItemList[i].requireCount}";

            }
        }

        void Hide()
        {
            gameObject.SetActive(false);
            _resultSpriteRenderer.gameObject.SetActive(false);
        }
    }
}
