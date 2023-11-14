using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlueprintDisplayer : MonoBehaviour
{
    [SerializeField] GameObject _requireItemOrigin;

    Building _building;

    List<GameObject> _slotList = new List<GameObject>();
    List<SpriteRenderer> _slotImageList = new List<SpriteRenderer>();
    List<TextMeshPro> _slotCountList = new List<TextMeshPro>();

    private void Awake()
    {
        _building = GetComponentInParent<Building>();

        BuildingBlueprint blueprint = _building.Blueprint;

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
            _slotCountList[i].text = $"{blueprint.BlueprintItemList[i].currentCount}/{blueprint.BlueprintItemList[i].requireCount}";

            Vector3 slotLocalPos = new Vector3(-(blueprint.BlueprintItemList.Count - 1) / 2f * 1.2f + i * 1.2f, 0, 0);
            _slotList[i].transform.localPosition = slotLocalPos;
        }

        _building.ItemChangedHandler += OnBlueprintChanaged;
    }


    void OnBlueprintChanaged(bool isFinish)
    {
        if(isFinish)
        {
            Hide();
            return;
        }

        BuildingBlueprint blueprint = _building.Blueprint;
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
    }
}
