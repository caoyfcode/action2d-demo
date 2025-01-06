using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMap : Menu
{
    public Map map;

    void Update()
    {
        bool moveUp = Input.GetKey(KeyCode.W);
        bool moveDown = Input.GetKey(KeyCode.S);
        bool moveLeft = Input.GetKey(KeyCode.A);
        bool moveRight = Input.GetKey(KeyCode.D);

        float moveX = (moveLeft, moveRight) switch
        {
            (true, false) => -1,
            (false, true) => 1,
            _ => 0
        };
        float moveY = (moveUp, moveDown) switch
        {
            (true, false) => 1,
            (false, true) => -1,
            _ => 0
        };
        var move = new Vector2(moveX, moveY);
        map.Move(move);

        bool scaleBig = Input.GetKey(KeyCode.K);
        bool scaleSmall = Input.GetKey(KeyCode.J);
        float scaleAdd = (scaleBig, scaleSmall) switch
        {
            (true, false) => 1f,
            (false, true) => -1f,
            _ => 0
        };
        map.ChangeScale(scaleAdd * 2);
    }
}
