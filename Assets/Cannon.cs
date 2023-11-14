using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class Cannon : MonoBehaviour, IBuildingOption
{
    [SerializeField] Range _detectRange;
    Character _target;

    [SerializeField] GameObject _cannonPos;
    [SerializeField] GameObject _firePoint;

    [SerializeField] ProjectileName _fireProjectileName;

    [SerializeField] float _coolTime;
    float _time;

    float _rotation;

    public void Init()
    {
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position + _detectRange.center, _detectRange.size);
    }
    void Update()
    {
        if (Client.Instance.IsSingle || Client.Instance.IsMain)
        {
            Detect();
            ControlCannon();
            Fire();
        }
    }

    void Detect()
    {
        if (_target != null) return;

         _target = Util.GetGameObjectByPhysics<Character>(transform.position, _detectRange, Define.EnemyLayerMask);

    }
    void ControlCannon()
    {
        if(_target == null) return;

        Vector3 targetPos = _target.transform.position;
        targetPos.x += +_target.CharacterSize.center.x * _target.transform.localScale.x;
        targetPos.y += +_target.CharacterSize.center.y;

        _rotation = Mathf.Atan2((targetPos.y - _cannonPos.transform.position.y), Mathf.Abs(targetPos.x - _cannonPos.transform.position.x));
        _rotation = _rotation / Mathf.PI * 180f;

        _cannonPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, _rotation));
    }

    void Fire()
    {
        _time += Time.deltaTime;
        if(_coolTime <= _time && _target != null)
        {
            Projectile projectile = null;
            Manager.Projectile.GenerateProjectile(_fireProjectileName, _firePoint.transform.position, ref projectile);

            if (projectile != null)
            {
                float rotation = _cannonPos.transform.eulerAngles.z;
                Vector3 direction = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad) * transform.localScale.x, Mathf.Sin(rotation * Mathf.Deg2Rad)).normalized;

                projectile.transform.position = _firePoint.transform.position;

                projectile.Fire(direction, Define.CharacterTag.Enemy, Define.CharacterTag.Enemy,3);
            }
            _time = 0;
        }
    }

    public void SerializeData()
    {
        Util.WriteSerializedData(_rotation);
    }

    public void DeserializeData()
    {
        _rotation = Util.ReadSerializedDataToFloat();
        _cannonPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, _rotation));

    }

    public void SerializeControlData()
    {
    }

    public void DeserializeControlData()
    {
    }
}
