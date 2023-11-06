using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Item : MonoBehaviour, IDataSerializable
{
    ItemData _itemData;
    public ItemData ItemData => _itemData;

    int _itemNumber;
    public int ItemId => _itemNumber;

    public bool IsHide { get; private set; }
    public bool IsGraped { set; get; }
    int _grapedCharacterId = -1;

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


        if(!Client.Instance.IsSingle  && Client.Instance.IsMain)
        {
            StartCoroutine(CorSendPacket());
        }
    }

    public void GrapItem(CustomCharacter character)
    {
        FreezeRigidBody();
        transform.parent = character.LiftPos.transform;
        transform.localPosition = Vector3.zero;
        IsGraped = true;
        _grapedCharacterId = character.CharacterId;

        Client.Instance.SendItemInfo(this);
    }
    public void GrapItem(int id)
    {
        CustomCharacter customCharacter = Manager.Character.GetCharacter(id) as CustomCharacter;

        if (customCharacter != null)
        {
            FreezeRigidBody();
            transform.parent = customCharacter.LiftPos.transform;
            transform.localPosition = Vector3.zero;
            IsGraped = true;
        }

        Client.Instance.SendItemInfo(this);
    }

    public void ReleaseItem(Character character = null, bool putDown = false)
    {
        ReleaseRigidBody();
        IsGraped = false;
        transform.parent = null;
        _grapedCharacterId = -1;

        if (putDown)
        {
                transform.position = character.transform.position;
        }

        Client.Instance.SendItemInfo(this);
    }

    public void FreezeRigidBody()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;

        Client.Instance.SendItemInfo(this);
    }

    public void ReleaseRigidBody()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = false;

        Client.Instance.SendItemInfo(this);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        IsHide = true;

        Client.Instance.SendItemInfo(this);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        IsHide = false;

        Client.Instance.SendItemInfo(this);
    }

    public void UpBound(float power)
    {
        _rigidbody.AddForce(Vector2.up* power,ForceMode2D.Impulse);
    }

    public string SerializeData()
    {
        Util.StartWriteSerializedData();

        Util.WriteSerializedData(_rigidbody.velocity.x);
        Util.WriteSerializedData(_rigidbody.velocity.y);
        Util.WriteSerializedData(_rigidbody.isKinematic);
        Util.WriteSerializedData(IsHide);
        Util.WriteSerializedData(_grapedCharacterId);
        Util.WriteSerializedData(IsGraped);

        return Util.EndWriteSerializeData();
    }

    public void DeserializeData(string stringData)
    {
        if (stringData.Equals(string.Empty)) return;

        Util.StartReadSerializedData(stringData);

        _rigidbody.velocity = new Vector3(Util.ReadSerializedDataToFloat(),Util.ReadSerializedDataToFloat());
        _rigidbody.isKinematic = Util.ReadSerializedDataToBoolean();
        bool isHide = Util.ReadSerializedDataToBoolean();
        if(IsHide && !isHide)
            Show();
        if(!IsHide && isHide)
            Hide();

        int id = Util.ReadSerializedDataToInt();
        bool isGraped = Util.ReadSerializedDataToBoolean();
        if (!IsGraped && isGraped)
        {
            GrapItem(id);
        }
        else if(IsGraped && !isGraped)
        {
            ReleaseItem();
        }
        IsGraped = isGraped;

    }

    public void SyncItemInfo(S_BroadcastItemInfo packet)
    {
        if (packet == null || Client.Instance.IsMain) return;

        DeserializeData(packet.data);

        if(!IsGraped)
        {
            transform.position = new Vector3(packet.posX, packet.posY);
        }
    }

    IEnumerator CorSendPacket()
    {
        while (true)
        {
            if(Mathf.Abs(_rigidbody.velocity.x) > 0.01f || Mathf.Abs(_rigidbody.velocity.y) > 0.01f)
            {
                Client.Instance.SendItemInfo(this);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}