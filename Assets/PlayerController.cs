using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Character _character;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Update()
    {
        Control();
    }


    void Control()
    {
        Vector2 moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.RightArrow))
            moveDirection.x = 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            moveDirection.x = -1;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _character.Jump();
        }
        _character.SetCharacterDirection(moveDirection);
    }

}
