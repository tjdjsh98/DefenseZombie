using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Define;

public class Arrow : Projectile
{
    [SerializeField] float power = 10;
    [SerializeField] EffectName _effect;
    private string _tag2;

    bool _isHit = false;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            if (_isHit) return;

            if (collision.gameObject.tag == _tag || collision.gameObject.tag == _tag2)
            {
                Character enemy = collision.gameObject.GetComponent<Character>();
                Building building = collision.gameObject.GetComponent<Building>();

                enemy?.Damage(1, transform.localScale.x > 0 ? Vector2.right : Vector2.left, 1, .5f);
                building?.Damage(1);
                _isHit = true;

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

    private void Update()
    {
        Vector3 dir = _rigidbody.velocity;
        float rotation = Mathf.Atan2(dir.y, dir.x);
        rotation = rotation / Mathf.PI * 180f;

        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));

    }

    public bool PredictTrajectory(Vector3 startPos, Vector3 vel, Vector3 dest)
    {
        int step = 120;
        float deltaTime = Time.fixedDeltaTime;
        Vector3 gravity = Physics.gravity;

        Vector3 position = startPos;
        Vector3 velocity = vel.normalized * power;

        for(int i = 0; i < step; i++)
        {
            position += (velocity * deltaTime) + (0.5f * gravity * deltaTime * deltaTime);
            velocity += gravity * deltaTime;

            if((dest - position).magnitude < 0.15f)
            {
                return true;
            }
        }

        return false;
    }

    public override void Fire(Vector3 direction, CharacterTag attackTag1, CharacterTag attackTag2)
    {
        _tag = attackTag1.ToString();
        _tag2 = attackTag2.ToString();

        _rigidbody.velocity = direction.normalized * power;

    }

}
