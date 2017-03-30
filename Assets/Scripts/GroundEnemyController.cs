using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroundEnemyController : MonoBehaviour
{

    public int[] pos;
    public int directionX { get; private set; }
    public int directionY { get; private set; }

    public GameManager gameMan;

    void Start()
    {
        do directionX = Random.Range(-1, 2); while (directionX == 0);
        do directionY = Random.Range(-1, 2); while (directionY == 0);
        if (pos[0] == gameMan.grid_screen_width && directionX == 1 || pos[0] == 1 && directionX == -1 ||
            gameMan.GetCellStatus(pos[0] + directionX, pos[1]) == "water") directionX *= -1;
        if (pos[1] == gameMan.grid_screen_height && directionY == 1 || pos[1] == 1 && directionY == -1 ||
            gameMan.GetCellStatus(pos[0], pos[1] + directionY) == "water") directionY *= -1;
    }

    void FixedUpdate()
    {
        gameObject.transform.position += Vector3.right * directionX * (gameMan.distanseStep / 10) * 2.5f;
        gameObject.transform.position += Vector3.down * directionY * (gameMan.distanseStep / 10) * 2.5f;

        if (directionX == 1 && gameObject.transform.localPosition.x >= gameMan.GetCellPosition(pos[0] + 1, pos[1]).x
            || directionX == -1 && gameObject.transform.localPosition.x <= gameMan.GetCellPosition(pos[0] - 1, pos[1]).x)
        {
            pos[0] += directionX;
            if (pos[0] == gameMan.grid_screen_width && directionX == 1 || pos[0] == 1 && directionX == -1
                || gameMan.GetCellStatus(pos[0] + directionX, pos[1] + directionY) == "water"
                || gameMan.GetCellStatus(pos[0] + directionX, pos[1] + directionY) == "track") directionX *= -1;
        }

        if (directionY == 1 && gameObject.transform.localPosition.y <= gameMan.GetCellPosition(pos[0], pos[1] + 1).y
            || directionY == -1 && gameObject.transform.localPosition.y >= gameMan.GetCellPosition(pos[0], pos[1] - 1).y)
        {
            pos[1] += directionY;
            if (pos[1] == gameMan.grid_screen_height && directionY == 1 || pos[1] == 1 && directionY == -1
                || gameMan.GetCellStatus(pos[0] + directionX, pos[1] + directionY) == "water"
                || gameMan.GetCellStatus(pos[0] + directionX, pos[1] + directionY) == "track") directionY *= -1;
        }
    }
}


