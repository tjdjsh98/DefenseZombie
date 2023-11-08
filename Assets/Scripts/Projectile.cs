using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Projectile : MonoBehaviour
{
    protected bool _isFire = false;
    protected Rigidbody2D _rigidbody;

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
        Destroy(gameObject);
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


    void Update()
    {
        if (_isFire)
        {
            transform.transform.position += transform.right * 10 * Time.deltaTime;
        }
    }

    public virtual void Throw(Vector3 direction, float power)
    {
        _rigidbody.AddForce(direction.normalized * power,ForceMode2D.Impulse);
    }
    public virtual void Fire(float direction, Vector3 rotation, CharacterTag attackTag)
    {
        _isFire= true;
        _tag = attackTag.ToString();
        rotation.z -= direction< 0 ? 180 : 0;

        transform.rotation = Quaternion.Euler(rotation);
        Vector3 scale = Vector3.one;
        scale.x = direction > 0 ? 1 : -1;
        transform.localScale=scale;
    }
}
