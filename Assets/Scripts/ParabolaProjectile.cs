using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class ParabolaProjectile : MonoBehaviour
{
    Vector3 _initPosition;
    Vector3 _destinationPosition;
    float _height;
    bool _isStart = false;
    float _speed;

    Rigidbody2D _rigidBody;

    float dh, mh, g, vy, dat, vx,mht = 1.5f,t;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision?.gameObject.tag == "Player")
        {
            Character playerCharacter = collision.gameObject.GetComponent<Character>();
            playerCharacter?.Damage(1, transform.localScale.x > 0 ? Vector2.right : Vector2.left, 1, .5f);

            if (playerCharacter != null) Destroy(gameObject);
        }
        else if(collision?.gameObject.tag == "Building")
        {
            Building building = collision.gameObject.GetComponent<Building>();
            building?.Damage(1);

            if (building != null) Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        FlyToDestination();
    }

    void FlyToDestination()
    {
        if (!_isStart) return;

        t += Time.fixedDeltaTime * _speed;

        if (t > dat) Destroy(gameObject);
        float x = _initPosition.x + vx * t;
        float y = _initPosition.y + vy * t - 0.5f*g*t*t;

        transform.position = new Vector2(x, y);
    }


    public void StartAttack(Vector3 destination,float speed, float height)
    {
        _initPosition = transform.position;
        _destinationPosition = destination;
        _height = height;
        _isStart = true;
        _speed = speed;

        dh = _destinationPosition.y - _initPosition.y;
        mh = _height;

        g = 2 * mh / (mht * mht);

        vy = Mathf.Sqrt(2 * g * mh);

        float a = g;
        float b = -2 * vy;
        float c = 2 * dh;

        dat = (-b + Mathf.Sqrt(b * b -4 * a * c)) / (2 * a);
        vx = -(_initPosition.x - _destinationPosition.x) / dat;
    }

    public Vector3 GetDirection(Vector3 destination, float speed, float height)
    {
        _initPosition = transform.position;
        _destinationPosition = destination;
        _height = height;
        _isStart = true;
        _speed = speed;

        dh = _destinationPosition.y - _initPosition.y;
        mh = _height;

        g = 2 * mh / (mht * mht);

        vy = Mathf.Sqrt(2 * g * mh);

        float a = g;
        float b = -2 * vy;
        float c = 2 * dh;

        dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        vx = -(_initPosition.x - _destinationPosition.x) / dat;

        return new Vector3(vx, vy, 0);
    }
}
