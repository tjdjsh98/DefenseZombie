using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Define;

public class Arrow : Projectile
{
    protected override void Awake()
    {
        base.Awake();
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

}
