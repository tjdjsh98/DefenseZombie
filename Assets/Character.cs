using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Rigidbody2D _rigidBody;

    public CharacterState CharacterState { set; get; }

    float _currentSpeed;
    [SerializeField]float _accelSpeed = 2.0f;
    [SerializeField] float _breakSpeed = 5.0f;

    [SerializeField] float _speed = 5.0f;
    [SerializeField] float _jumpPower = 10.0f;
    Vector2 _characterMoveDirection;

    public Action<float> TurnHandler;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        MoveCharacter();
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

    public void SetCharacterDirection(Vector2 moveDirection)
    {
        _characterMoveDirection = moveDirection;
    }
    public void Jump()
    {
        _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
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
    }

    void TurnToIdle()
    {
        CharacterState = CharacterState.Idle;
    }
}

public enum CharacterState
{
    Idle,
    Damage
}