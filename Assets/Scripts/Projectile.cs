using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Projectile : MonoBehaviour
{
    [SerializeField] ProjectileName _projectileName;
    public ProjectileName ProjectileName => _projectileName;
    [SerializeField] ProjectileType _projectileType;
    public ProjectileType ProjectileType =>_projectileType;

    protected bool _isFire = false;
    protected Rigidbody2D _rigidbody;

    [SerializeField] protected float power = 10;
    [SerializeField] protected EffectName _effect;
    protected string _tag1;
    protected string _tag2;

    public int ProjectileId;

    protected bool _isHit = false;

    [SerializeField] protected bool _autoDestroy;
    [SerializeField] protected float _destroyTime;


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
        if (Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            if (_isHit) return;

            if (collision.gameObject.tag == _tag1 || collision.gameObject.tag == _tag2)
            {
                Character enemy = collision.gameObject.GetComponent<Character>();
                Building building = collision.gameObject.GetComponent<Building>();

                enemy?.Damage(1, transform.localScale.x > 0 ? Vector2.right : Vector2.left, 1, .5f);
                building?.Damage(1);
                _isHit = true;

                Debug.Log("!");

                if (enemy != null || building != null)
                {
                    collision.ClosestPoint(transform.position);
                    GameObject effect = null;
                    Manager.Effect.GenerateEffect(_effect, collision.ClosestPoint(transform.position), ref effect);
                    Manager.Projectile.RemoveProjectile(ProjectileId);
                }
            }
            if (_isDestroyOnGround && (1 << collision.gameObject.layer) == Define.GroundLayerMask)
            {
                GameObject effect = null;
                Manager.Effect.GenerateEffect(_effect, collision.ClosestPoint(transform.position), ref effect);
                Manager.Projectile.RemoveProjectile(ProjectileId);
                _isHit = true;
            }
        }
    }

    public virtual void Throw(Vector3 direction, float power)
    {
        _rigidbody.AddForce(direction.normalized * power,ForceMode2D.Impulse);
    }
    public virtual void Fire(Vector3 direction, CharacterTag attackTag1, CharacterTag attackTag2)
    {
        _tag1 = attackTag1.ToString();
        _tag2 = attackTag2.ToString();
        _rigidbody.velocity = direction * 10;
    }
}
