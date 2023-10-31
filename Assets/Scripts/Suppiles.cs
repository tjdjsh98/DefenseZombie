using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Suppiles : InteractableObject
{
    [SerializeField] List<ItemName> itemNameList;

    bool _isDropping = true;

    [SerializeField]Range _checkground;

    private void Update()
    {
        if (_isDropping) {
            GameObject ground = Util.GetGameObjectByPhysics(transform.position, _checkground, GroundLayerMask);

            if (ground !=null)
            {
                GameObject effectOrigin = Manager.Data.GetEffect(EffectName.Dust);

                GameObject effect= Instantiate(effectOrigin);
                effect.transform.position = transform.position;

                _isDropping = false;
            }
        }
    }
    public override bool Interact(Character character)
    {
        if (!_isDropping)
        {
            foreach (var item in itemNameList)
            {
                Manager.Item.GenerateItem(item, transform.position);
            }
            Manager.Item.DestroyItem(GetComponent<Item>().ItemId);
        }
        return false;
    }
}
