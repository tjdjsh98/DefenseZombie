using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    ItemData _itemData;
    public ItemData ItemData => _itemData;

    int _itemNumber;
    public int ItemId => _itemNumber;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rigidbody;

    public bool IsFreeze => _rigidbody.isKinematic;
    
    public void Init(ItemData itemData, int itemNumber)
    {
        _itemData = itemData;
        _itemNumber = itemNumber;

        _spriteRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();   
        _spriteRenderer.sprite = _itemData.ItemSprite;
    }

    public void FreezeRigidBody()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
    }

    public void ReleaseRigidBody()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = false;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void UpBound(float power)
    {
        _rigidbody.AddForce(Vector2.up* power,ForceMode2D.Impulse);
    }
}
