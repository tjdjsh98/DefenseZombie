using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RequireItemDisplayer : MonoBehaviour
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

        for (int i = 0; i < blueprint.BlueprintItmeList.Count; i++)
        {
            GameObject slot = Instantiate(_requireItemOrigin,transform);
            _slotImageList.Add(slot.transform.Find("Image").GetComponent<SpriteRenderer>());
            _slotCountList.Add(slot.transform.Find("Count").GetComponent<TextMeshPro>());

            ItemData itemData = Manager.Data.GetItemData(blueprint.BlueprintItmeList[i].name);
            _slotImageList[i].sprite = itemData.ItemSprite;
            _slotCountList[i].text = $"{blueprint.BlueprintItmeList[i].currentCount}/{blueprint.BlueprintItmeList[i].requireCount}";

            Vector3 slotLocalPos = new Vector3(-(blueprint.BlueprintItmeList.Count - 1) / 2f * 1.2f + i * 1.2f, 0, 0);
            slot.transform.localPosition = slotLocalPos;
        }

        _building.BlueprintChangedHandler += OnBlueprintChanaged;
    }


    void OnBlueprintChanaged(bool isFinish)
    {
        if(isFinish)
        {
            Hide();
            return;
        }

        BuildingBlueprint blueprint = _building.Blueprint;
        for (int i = 0; i < blueprint.BlueprintItmeList.Count; i++)
        {
            ItemData itemData = Manager.Data.GetItemData(blueprint.BlueprintItmeList[i].name);
            _slotImageList[i].sprite = itemData.ItemSprite;
            _slotCountList[i].text = $"{blueprint.BlueprintItmeList[i].currentCount}/{blueprint.BlueprintItmeList[i].requireCount}";

        }
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
