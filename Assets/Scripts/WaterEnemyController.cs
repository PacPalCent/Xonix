using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterEnemyController : MonoBehaviour
{
    public int[] pos;
    public int directionX { get; private set; }
    public int directionY { get; private set; }

    public GameManager gameMan;
    
    void Start ()
	{
	    do directionX = Random.Range(-1, 2);
        while (directionX == 0);
        do directionY = Random.Range(-1, 2);
        while (directionY == 0);
    }

    

    void FixedUpdate()
    {
        gameObject.transform.position += Vector3.right * directionX * (gameMan.distanseStep / 10) * 2;

        gameObject.transform.position += Vector3.up * directionY * (gameMan.distanseStep / 10) * 2;

            if (gameObject.transform.localPosition.x >= gameMan.GetCellPosition(pos[0] + 1, pos[1]).x && directionX == 1
                ||
                gameObject.transform.localPosition.x <= gameMan.GetCellPosition(pos[0] - 1, pos[1]).x &&
                directionX == -1)
            {
                pos[0] += directionX;
                if (gameMan.GetCellStatus(pos[0] + directionX, pos[1]) == "ground")
                {
                    directionX *= -1;
                }
                if (gameMan.GetCellStatus(pos[0] + directionX, pos[1]) == "track")
                {
                    gameMan.GameOver();
                }
            }
        
            if (gameObject.transform.localPosition.y >= gameMan.GetCellPosition(pos[0], pos[1] - 1).y && directionY == 1
                ||
                gameObject.transform.localPosition.y <= gameMan.GetCellPosition(pos[0], pos[1] + 1).y &&
                directionY == -1)
            {
                pos[1] -= directionY;
                if (gameMan.GetCellStatus(pos[0], pos[1] - directionY) == "ground")
                {
                    directionY *= -1;
                }
                if (gameMan.GetCellStatus(pos[0], pos[1] - directionY) == "track")
                {
                    gameMan.GameOver();
                }
            }
        
    }

}
