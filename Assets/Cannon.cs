using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Cannon : MonoBehaviour
{
    [SerializeField] Range _detectRange;
    Character _target;

    [SerializeField] GameObject _cannonPos;
    [SerializeField] GameObject _firePoint;
    [SerializeField] Projectile _projectile;


    [SerializeField] float _coolTime;
    float _time;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position + _detectRange.center, _detectRange.size);
    }

    void Update()
    {
        Detect();
        ControlCannon();
        Fire();
    }

    void Detect()
    {
        if (_target != null) return;

         List<Character> characterList = Util.GetGameObjectsByPhysics<Character>(transform.position, _detectRange, 1 << LayerMask.NameToLayer("Character"));

        foreach (Character character in characterList)
        {
            if(character.tag == "Enemy")
            {
                _target = character;
                return;
            }
        }
    }
    void ControlCannon()
    {
        if(_target == null) return;

        float rotation = Mathf.Atan2((_target.transform.position.y - _cannonPos.transform.position.y), Mathf.Abs(_target.transform.position.x - _cannonPos.transform.position.x));
        rotation = rotation / Mathf.PI * 180f;

        _cannonPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
    }

    void Fire()
    {
        _time += Time.deltaTime;
        if(_coolTime <= _time)
        {
            Projectile projectile = Instantiate(_projectile);
            projectile.transform.position = _firePoint.transform.position;
            projectile.Fire(transform.lossyScale.x, _cannonPos.transform.eulerAngles);
            _time = 0;
        }
    }
}
