using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int minSwipeDictanse = 50;
    private Vector2 swipeStartPos;

    private int[] startPos;
    private int[] pos = new int[2];

    private int currentDirectionX;
    private int currentDirectionY;
    private int nextDirectionX;
    private int nextDirectionY;
    
    public bool inWater { get; private set; }

    public GameManager gameMan;
	
	void Start ()
	{
	    startPos = new[] {gameMan.grid_screen_width / 2, 2};
        StartPosition();
    }

    public void StartPosition()
    {
        startPos.CopyTo(pos,0);
        currentDirectionX = 0;
        currentDirectionY = 0;
        nextDirectionX = 0;
        nextDirectionY = 0;
        inWater = false;
        gameObject.GetComponent<RectTransform>().localPosition = new Vector2(gameMan.GetCellPosition(startPos[0],startPos[1]).x, gameMan.GetCellPosition(startPos[0], startPos[1]).y);
    }

    public int[] GetPosition()
    {
        return pos;
    }

    public void NewDirection(int directionX, int  directionY)
    {
        
        if (currentDirectionX == 0 && currentDirectionY == 0)
        {
            currentDirectionX = directionX;
            currentDirectionY = directionY;
        }
        else
        {
            if (currentDirectionX == -directionX && currentDirectionX != 0|| currentDirectionY == -directionY && currentDirectionY !=0)
            {
                gameMan.GameOver();
            }
            else
            {
                nextDirectionX = directionX;
                nextDirectionY = directionY;
            }
        }
    }

    void FixedUpdate()
    {
        if (currentDirectionX == 1 && gameObject.transform.localPosition.x >= gameMan.GetCellPosition(pos [0], pos [1]).x
            || currentDirectionX == -1 && gameObject.transform.localPosition.x <= gameMan.GetCellPosition(pos [0], pos [1]).x
            || currentDirectionY == 1 && gameObject.transform.localPosition.y <= gameMan.GetCellPosition(pos [0], pos [1]).y
            || currentDirectionY == -1 && gameObject.transform.localPosition.y >= gameMan.GetCellPosition(pos [0], pos [1]).y)
        {
            if (gameMan.GetCellStatus(pos[0], pos[1]) == "water" && !inWater)
            {
                inWater = true;
            }
            if (gameMan.GetCellStatus(pos[0], pos[1]) == "ground" && inWater)
            {
                inWater = false;
                gameMan.FillNewGround();
                currentDirectionX = 0;
                currentDirectionY = 0;
            }
            if (gameMan.GetCellStatus(pos[0], pos[1]) == "water") gameMan.ChangeColorMove(pos[0], pos[1]);



            if (nextDirectionY != 0 || nextDirectionX != 0)
            {
                currentDirectionX = nextDirectionX;
                currentDirectionY = nextDirectionY;
                nextDirectionX = 0;
                nextDirectionY = 0;
            }
            else
            {
                if (pos[0] == 1 && currentDirectionX == -1 || pos[0] == gameMan.grid_screen_width && currentDirectionX == 1)
                {
                    currentDirectionX = 0;
                }
                else pos[0] += currentDirectionX;

                if (pos[1] == 1 && currentDirectionY == -1 || pos[1] == gameMan.grid_screen_height && currentDirectionY == 1)
                {
                    currentDirectionY = 0;
                }
                else pos[1] += currentDirectionY;

                if (gameMan.GetCellStatus(pos[0], pos[1]) == "track")
                {
                    gameMan.GameOver();
                }
            }
        }
        for (var i = 0; i < gameMan.GroundEnemy.Length; i++)
        {
            if (gameObject.transform.position.x + gameMan.distanseStep > gameMan.GroundEnemy[i].gameObject.transform.position.x &&
                gameObject.transform.position.x - gameMan.distanseStep < gameMan.GroundEnemy[i].gameObject.transform.position.x &&
                gameObject.transform.position.y + gameMan.distanseStep > gameMan.GroundEnemy[i].gameObject.transform.position.y &&
                gameObject.transform.position.y - gameMan.distanseStep < gameMan.GroundEnemy[i].gameObject.transform.position.y)
            {
                gameMan.GameOver();
            }
        }
        for (var i = 0; i < gameMan.WaterEnemy.Length; i++)
        {
            if (gameMan.WaterEnemy[i] &&
                gameObject.transform.position.x + gameMan.distanseStep > gameMan.WaterEnemy[i].gameObject.transform.position.x &&
                gameObject.transform.position.x - gameMan.distanseStep < gameMan.WaterEnemy[i].gameObject.transform.position.x &&
                gameObject.transform.position.y + gameMan.distanseStep > gameMan.WaterEnemy[i].gameObject.transform.position.y &&
                gameObject.transform.position.y - gameMan.distanseStep < gameMan.WaterEnemy[i].gameObject.transform.position.y)
            {
                gameMan.GameOver();
            }
        }

        gameObject.transform.position += Vector3.right * currentDirectionX * (gameMan.distanseStep / 10) * 2.5f;
        gameObject.transform.position += Vector3.down * currentDirectionY * (gameMan.distanseStep / 10) * 2.5f;
    }

    void Update () {
	    if (!Application.isMobilePlatform)
	    {
	        if (Input.GetKeyDown(KeyCode.DownArrow)) NewDirection(0, 1);
	        if (Input.GetKeyDown(KeyCode.UpArrow)) NewDirection(0, -1);
	        if (Input.GetKeyDown(KeyCode.LeftArrow)) NewDirection(-1, 0);
	        if (Input.GetKeyDown(KeyCode.RightArrow)) NewDirection(1, 0);
	    }
	    else
	    {
	        if (Input.touchCount > 0)
	        {
	            Touch currentTouch = Input.touches[0];
	            if (currentTouch.phase == TouchPhase.Began)
	            {
	                swipeStartPos = currentTouch.position;
	            }
	            if (currentTouch.phase == TouchPhase.Ended)
	            {
                    float swipeDistHorizontal =
                        (new Vector2(currentTouch.position.x, 0) - new Vector2(swipeStartPos.x, 0)).magnitude;
                    if (swipeDistHorizontal >= minSwipeDictanse)
                    {
                        float SwipeValue = Mathf.Sign(currentTouch.position.x - swipeStartPos.x);
                        if (SwipeValue > 0) NewDirection(1, 0);
                        else if (SwipeValue < 0) NewDirection(-1, 0);
                    }
                    float swipeDistVertical =
	                    (new Vector2(0, currentTouch.position.y) - new Vector2(0, swipeStartPos.y)).magnitude;
	                if (swipeDistVertical >= minSwipeDictanse)
	                {
	                    float SwipeValue = Mathf.Sign(currentTouch.position.y - swipeStartPos.y);
	                    if (SwipeValue > 0) NewDirection(0, -1);
                        else if(SwipeValue<0) NewDirection(0, 1);
                    }
	            }
	        }
	    }
        //Debug.Log(gameObject.transform.position);
    }

    
}
