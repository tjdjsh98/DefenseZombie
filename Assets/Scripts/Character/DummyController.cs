using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    Character _character;
    Vector3 _nextMove;

    Vector3 _prePos;
    S_BroadcastMove _prePacket;
    S_BroadcastMove _currentPacket;

    float _time = 0;
    float _interval = 0.02f;

    void Awake()
    {
        _character = GetComponent<Character>();
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
                transform.position += distanceInterval;
            }
            else
            {
                _currentPacket = null;
            }
        }
    }

    public void SetMovePacket(S_BroadcastMove packet)
    {
        _interval = _time;
        _time = 0;

        _character.SetCharacterDirection(new Vector2(packet.characterMoveDirection, 0));
        _character.SetCurrentSpeed (packet.currentSpeed);
        _character.SetVelocity(new Vector2(packet.currentSpeed, packet.ySpeed));
        _character.AttackType = packet.attackType;
        _character.IsJumping = packet.isJumping;
        _character.IsAttacking = packet.isAttacking;
        _character.IsConncetCombo = packet.isConnectCombo;

        if (_interval == 0)
        {
            Vector3 currentPos = new Vector3(packet.posX, packet.posY, packet.posZ);
            transform.position = currentPos;
        }
        else
        {
            _prePos = transform.position;
            _currentPacket = packet;
        }
    }
}
