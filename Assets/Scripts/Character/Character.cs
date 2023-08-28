using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    Rigidbody2D _rigidBody;

    [SerializeField]Range _characterSize;

    public CharacterState CharacterState { set; get; }
    Vector2 _characterMoveDirection;

    float _currentSpeed;
    [SerializeField]float _accelSpeed = 2.0f;
    [SerializeField] float _breakSpeed = 5.0f;
    [SerializeField] float _speed = 5.0f;

    [SerializeField] Range _range;
    [SerializeField] float _jumpPower = 10.0f;
    int _jumpCount = 0;
    
    public Action<float> TurnHandler;

    [SerializeField] int _maxHp;
    int _hp;
    public int Hp { get { return _hp; } set { _hp = value; if (_hp <= 0) Dead(); } }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Hp = _maxHp;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(transform.position + _characterSize.center , _characterSize.size);
    }

    void Update()
    {
        MoveCharacter();
        CheckGround();

    }
    void MoveCharacter()
    {
        if (CharacterState != CharacterState.Idle)
        {
            _currentSpeed = _rigidBody.velocity.x;
            if (_currentSpeed < 0)
            {
                _currentSpeed += _breakSpeed * Time.deltaTime;
                if (_currentSpeed > 0)
                    _currentSpeed = 0;
            }
            if (_currentSpeed > 0)
            {
                _currentSpeed -= _breakSpeed * Time.deltaTime;
                if (_currentSpeed < 0)
                    _currentSpeed = 0;
            }

            _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
            return;
        }
        if(_characterMoveDirection.x == 0)
        {
            if (_currentSpeed < 0)
            {
                _currentSpeed += _breakSpeed * Time.deltaTime;
                if (_currentSpeed > 0)
                    _currentSpeed = 0;
            }
            if (_currentSpeed > 0)
            {
                _currentSpeed -= _breakSpeed * Time.deltaTime;
                if (_currentSpeed < 0)
                    _currentSpeed = 0;
            }
        }
        else if (_characterMoveDirection.x > 0)
        {
            if (_currentSpeed < 0)
            {
                _currentSpeed += _breakSpeed * Time.deltaTime;
            }
            else
            {
                Turn(_currentSpeed);
                _currentSpeed += _accelSpeed * Time.deltaTime;
            }

            if(_currentSpeed > 5)
                _currentSpeed = 5;
        }
        else if(_characterMoveDirection.x < 0)
        {
            if (_currentSpeed > 0)
            {
                _currentSpeed -= _breakSpeed * Time.deltaTime;
            }
            else
            {
                Turn(_currentSpeed);
                _currentSpeed -= _accelSpeed * Time.deltaTime;
            }

            if (_currentSpeed < -5)
                _currentSpeed = -5;
        }

        _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
    }

    void CheckGround()
    {

    }

    public void SetCharacterDirection(Vector2 moveDirection)
    {
        _characterMoveDirection = moveDirection;
    }
    public void Jump()
    {
        if(_jumpCount == 0)
        {
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            _jumpCount++;
        }
    }

    void Turn(float direction)
    {
        if(TurnHandler != null)
            TurnHandler(direction);

        Vector3 scale = Vector3.one;
        scale.x = direction > 0 ? 1 : -1;
        transform.localScale = scale;
    }

    public void Damage(int dmg, Vector2 attackDirection, float power)
    {
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.AddForce(attackDirection * power, ForceMode2D.Impulse);
        CharacterState = CharacterState.Damage;
        Invoke("TurnToIdle", 1f);

        Hp -= dmg;
     
    }

    void Dead()
    {
        Destroy(this.gameObject);
    }

    void TurnToIdle()
    {
        CharacterState = CharacterState.Idle;
    }

    public GameObject GetOverrapGameObject(int layerMask = -1)
    {
        GameObject gameObject = null;

        gameObject = Util.GetGameObjectByPhysics(transform.position, _characterSize, layerMask);

        return gameObject;
    }

    public T GetOverrapGameObject<T>(int layerMask = -1) where T : MonoBehaviour
    {
        T gameObject = null;

        gameObject = Util.GetGameObjectByPhysics<T>(transform.position ,_characterSize, layerMask);

        return gameObject;
    }
}

public enum CharacterState
{
    Idle,
    Damage
}