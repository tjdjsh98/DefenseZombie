using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Projectile : MonoBehaviour
{
    protected bool _isFire = false;
    protected Rigidbody2D _rigidbody;

    public int ProjectileId;

    [SerializeField] ProjectileName _projectileName;
    public ProjectileName ProjectileName => _projectileName;
    [SerializeField] ProjectileType _projectileType;
    public ProjectileType ProjectileType =>_projectileType;

    [SerializeField] protected bool _autoDestroy;
    [SerializeField] protected float _destroyTime;

    protected string _tag;

    protected bool _isDestroyOnGround = true;

    protected virtual void Awake()
    {
        _rigidbody= GetComponent<Rigidbody2D>();
        if(_autoDestroy)
            Invoke("Destroy", _destroyTime);
    }
    void Destroy()
    {
        Manager.Projectile.RemoveProjectile(ProjectileId);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == _tag)
        {
            Character enemy = collision.gameObject.GetComponent<Character>();
            enemy?.Damage(1, transform.localScale.x > 0 ? Vector2.right : Vector2.left, 1, .5f);

            if (enemy != null) Destroy(gameObject);
        }
    }

    public virtual void Throw(Vector3 direction, float power)
    {
        _rigidbody.AddForce(direction.normalized * power,ForceMode2D.Impulse);
    }
    public virtual void Fire(Vector3 direction, CharacterTag attackTag1, CharacterTag attackTag2)
    {
        _rigidbody.velocity = direction * 10;
    }
}
