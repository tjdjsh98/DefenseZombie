using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Coroutine _shakeEffect;
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (Manager.Game.Commander.gameObject.activeSelf) return;
        if (Manager.Character.MainCharacter == null || _shakeEffect != null) return;

        
        Vector3 lerpPosition = Vector3.Lerp(transform.position, Manager.Character.MainCharacter.transform.position
            + Vector3.up *2, 0.05f);

        lerpPosition.z = -10;
        transform.position = lerpPosition;
            
    }

    public void ShakeCamera(float power,float time)
    {
        _shakeEffect = StartCoroutine(CorShakeCamera(power,time));
    }


    IEnumerator CorShakeCamera(float power, float time)
    {
        float currentTime = 0;
        while (currentTime < time)
        {
            currentTime += Time.fixedDeltaTime;
            Vector3 lerpPosition = Vector3.zero;
            if (Manager.Character.MainCharacter != null)
                lerpPosition = Manager.Character.MainCharacter.transform.position + Vector3.up * 2;
            else
                lerpPosition = transform.position;

            lerpPosition.z = -10;

            lerpPosition += (Vector3)Random.insideUnitCircle * Mathf.Log(power, 10000);

            transform.position = lerpPosition;
            yield return new WaitForFixedUpdate();
        }
        _shakeEffect = null;
    }
}
