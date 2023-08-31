using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (Manager.Character.MainCharacter == null) return;
        Vector3 lerpPosition = Vector3.Lerp(transform.position, Manager.Character.MainCharacter.transform.position
            + Vector3.up *2, 0.05f);
        lerpPosition.z = -10;
        transform.position = lerpPosition;
            
    }
}
