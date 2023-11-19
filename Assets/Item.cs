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

    public bool IsGraped { set; get; }
    int _grapedCharacterId = -1;
    public int GrapedCharacterId => _grapedCharacterId;

    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rigidbody;

    public bool IsFreeze => _rigidbody.isKinematic;
    
    List<IItemOption> _optionList=  new List<IItemOption>();

    public void Init(ItemData itemData, int itemNumber)
    {
        _itemData = itemData;
        _itemNumber = itemNumber;

        _spriteRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();   

        _spriteRenderer.sprite = _itemData.ItemSprite;

        IItemOption[] itemOptions = GetComponents<IItemOption>();

        foreach (var o in itemOptions)
        {
            o.Init();
            _optionList.Add(o);
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
            _grapedCharacterId = id;
            IsGraped = true;
        }

        Client.Instance.SendItemInfo(this);
    }

    public void ReleaseItem(Character character = null, bool putDown = false)
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = false;
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

        Client.Instance.SendItemInfo(this);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        Client.Instance.SendItemInfo(this);
    }

  
    public void RandomBounding(float power)
    {
        Vector3 bound = Vector2.up;
        bound.x = Random.Range(-0.5f, 0.5f);
        bound = bound.normalized;

        _rigidbody.AddForce(bound * power,ForceMode2D.Impulse);

        Client.Instance.SendItemInfo(this);
    }

    public string SerializeData()
    {
        Util.StartWriteSerializedData();

        Util.WriteSerializedData(_rigidbody.velocity.x);
        Util.WriteSerializedData(_rigidbody.velocity.y);
        Util.WriteSerializedData(_rigidbody.isKinematic);
        Util.WriteSerializedData(gameObject.activeSelf);
        Util.WriteSerializedData(_grapedCharacterId);
        Util.WriteSerializedData(IsGraped);

        foreach (var option in _optionList)
        {
            option.SerializeData();
        }
      
        return Util.EndWriteSerializeData();
    }

    public void DeserializeData(string stringData)
    {
        if (stringData.Equals(string.Empty)) return;

        Util.StartReadSerializedData(stringData);

        _rigidbody.velocity = new Vector3(Util.ReadSerializedDataToFloat(),Util.ReadSerializedDataToFloat());
        _rigidbody.isKinematic = Util.ReadSerializedDataToBoolean();
        bool active = Util.ReadSerializedDataToBoolean();
        gameObject.SetActive(active);
        

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
        foreach (var option in _optionList)
        {
            option.DeserializeData();
        }
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

  
}