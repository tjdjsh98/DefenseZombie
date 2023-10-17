using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool _isFire = false;

    void Awake()
    {
        Invoke("Destroy", 3f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
            enemy?.Damage(1, transform.localScale.x > 0 ? Vector2.right : Vector2.left, 1, .5f);

            if (enemy != null) Destroy(gameObject);
        }
    }


    void Update()
    {
        if (_isFire)
        {
            transform.transform.position += transform.right * 10 * Time.deltaTime;
        }
    }

    public void Fire(float direction, Vector3 rotation)
    {
        _isFire= true;

        rotation.z -= direction< 0 ? 180 : 0;

        transform.rotation = Quaternion.Euler(rotation);
        Vector3 scale = Vector3.one;
        scale.x = direction > 0 ? 1 : -1;
        transform.localScale=scale;
    }
}
