using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField] float power = 10;

    protected override void Awake()
    {
        base.Awake();
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

    public void Fire(Vector3 direction)
    {
        _rigidbody.velocity = direction.normalized * power;
    }

}
