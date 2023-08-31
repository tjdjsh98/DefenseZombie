using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Character _character;
    House _inHouse;

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

        if(Input.GetKeyDown(KeyCode.S))
        {
            if(_inHouse == null)
            {
                House house = _character.GetOverrapGameObject<House>();
                if (house != null)
                {
                    house.EnterCharacter(_character);
                    _inHouse = house;
                }
            }
            else
            {
                _inHouse.LeaveCharacter();
                _inHouse = null;
            }
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            Manager.Building.GenreateBuilding();
                Manager.Building.StartBuildingDraw(this.gameObject, "CommanderHouse");

        }

        _character.SetCharacterDirection(moveDirection);
    }

}
