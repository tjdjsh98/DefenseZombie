using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class DummyCharacter : Character
{
    public int CharacterId { set; get; }
    Vector3 _nextMove;

    Vector3 _prePos;
    S_BroadcastMove _prePacket;
    S_BroadcastMove _currentPacket;

    float _time = 0;
    float _interval = 0.02f;
    protected override void MoveCharacter()
    {
        if (CharacterState != CharacterState.Idle)
        {
            _currentSpeed = _rigidBody.velocity.x;
            if (_currentSpeed < 0)
            {
                _currentSpeed += BreakSpeed * Time.deltaTime;
                if (_currentSpeed > 0)
                    _currentSpeed = 0;
            }
            if (_currentSpeed > 0)
            {
                _currentSpeed -= BreakSpeed * Time.deltaTime;
                if (_currentSpeed < 0)
                    _currentSpeed = 0;
            }

            _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
            return;
        }
        if (_characterMoveDirection.x == 0)
        {
            if (_currentSpeed < 0)
            {
                _currentSpeed += BreakSpeed * Time.deltaTime;
                if (_currentSpeed > 0)
                    _currentSpeed = 0;
            }
            if (_currentSpeed > 0)
            {
                _currentSpeed -= BreakSpeed * Time.deltaTime;
                if (_currentSpeed < 0)
                    _currentSpeed = 0;
            }
        }
        else if (_characterMoveDirection.x > 0)
        {
            if (_currentSpeed < 0)
            {
                _currentSpeed += BreakSpeed * Time.deltaTime;
            }
            else
            {
                Turn(_currentSpeed);
                _currentSpeed += _accelSpeed * Time.deltaTime;
            }

            if (_currentSpeed > _speed)
                _currentSpeed = _speed;
        }
        else if (_characterMoveDirection.x < 0)
        {
            if (_currentSpeed > 0)
            {
                _currentSpeed -= BreakSpeed * Time.deltaTime;
            }
            else
            {
                Turn(_currentSpeed);
                _currentSpeed -= _accelSpeed * Time.deltaTime;
            }

            if (_currentSpeed < -_speed)
                _currentSpeed = -_speed;
        }
        if (_jumpCount == 0 && IsJumping)
        {
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            IsJumping = false;
            _jumpCount++;
        }
    }

    private void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;

        if (_currentPacket != null)
        {
            if (_interval > _time)
            {
                Vector3 currentPos = new Vector3(_currentPacket.posX, _currentPacket.posY, _currentPacket.posZ);
                Vector3 distanceInterval = new Vector3(currentPos.x - _prePos.x, currentPos.y - _prePos.y, 0);
                distanceInterval /= _interval * 60;
                Debug.Log(distanceInterval);
                transform.position += distanceInterval;
            }
            else
            {
                _currentPacket = null;
            }
        }
    }
    protected override void ControlAnimation()
    {
        if (CharacterState == CharacterState.Idle)
        {
            if(CharacterMoveDirection != Vector2.zero)
                Turn(CharacterMoveDirection.x);
            if (_currentSpeed != 0)
            {
                SetAnimatorBool("Walk", true);
            }
            else
            {
                SetAnimatorBool("Walk", false);
            }
        }

        SetAnimatorInteger("AttackType", AttackType);
        SetAnimatorBool("Attack", IsAttacking);

        SetAnimatorFloat("VelocityY", _ySpeed);


        SetAnimatorBool("ContactGround", IsContactGround);

        if (IsConncetCombo)
        {
            SetAnimatorTrigger("ConnectCombo");
            IsConncetCombo = false;
        }
    }

    public void SetMovePacket(S_BroadcastMove packet)
    {
        _interval = _time;
        _time = 0;


        _characterMoveDirection = new Vector2(packet.characterMoveDirection, 0);
        _currentSpeed = packet.currentSpeed;
        _rigidBody.velocity = new Vector2(packet.currentSpeed, packet.ySpeed);
        AttackType = packet.attackType;
        IsJumping = packet.isJumping;
        IsAttacking = packet.isAttacking;
        IsConncetCombo = packet.isConnectCombo;


        if (_interval == 0)
        {
            Vector3 currentPos = new Vector3(packet.posX, packet.posY, packet.posZ);
            Debug.Log(currentPos);
            transform.position = currentPos;
        }
        else
        {
            _prePos = transform.position;
            _currentPacket = packet;
        }
    }
}
