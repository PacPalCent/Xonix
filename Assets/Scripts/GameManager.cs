using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour {

    public int grid_screen_width { get; private set; }
    public int grid_screen_height { get; private set; }
    private int startSumWater;
    private int currentSumWater;
    public float distanseStep { get; private set; }
    //public GameObject SpawnEnemy;
    //public GameObject SpawnPlayer;
    //public GameObject GameGrid;
    private GameObject newObject;
    public RectTransform rectGameField;
    public RectTransform rectUI;
    public RectTransform rectCanvas;
    public Canvas MainCanvas;
    public WaterEnemyController[] WaterEnemy { get; private set; }
    public GroundEnemyController[] GroundEnemy { get; private set; }
    public Color[] colorCell;
    GridCell[,] GridElement;
    int biggestEnemyTerritory;

    private int level = 1;

    private int health;
    private float leftTime = 60;
    
    private List<int> SumCellInTerritory;

    public Text textHP;
    public Text textProgress;
    public Text textTime;
    
    bool InWater;

    PlayerController playerControl = new PlayerController();

    void Start()
    {
        grid_screen_width = 64;
        distanseStep = Screen.width / rectCanvas.lossyScale.x / grid_screen_width;
        grid_screen_height = (int) ((Screen.height / rectCanvas.lossyScale.y - rectUI.sizeDelta.y) / distanseStep);
        rectGameField.sizeDelta = new Vector2(rectGameField.sizeDelta.x, grid_screen_height*distanseStep);
        rectGameField.localPosition = new Vector2(rectGameField.localPosition.x, (rectGameField.sizeDelta.y - Screen.height / rectCanvas.lossyScale.y) /2);
        rectUI.sizeDelta = new Vector2(rectUI.sizeDelta.x, Screen.height / rectCanvas.lossyScale.y - rectGameField.sizeDelta.y);
        rectUI.localPosition = new Vector2(rectUI.localPosition.x, (-rectUI.sizeDelta.y + Screen.height / rectCanvas.lossyScale.y) / 2);

        rectGameField.transform.FindChild("GridLayer").GetComponent<GridLayoutGroup>().cellSize = new Vector2(distanseStep, distanseStep);
        GridElement = new GridCell[grid_screen_width + 1, grid_screen_height + 1];
        for (var i = 1; i <= grid_screen_width; i++)
        {
            for (var j = 1; j <= grid_screen_height; j++)
            {
                newObject = Instantiate(Resources.Load("GridElement", typeof(GameObject))) as GameObject;
                GridElement[i, j] = newObject.GetComponent<GridCell>();
                newObject.transform.SetParent(rectGameField.transform.FindChild("GridLayer"));
                newObject.transform.localScale = new Vector2(1, 1);
                newObject.name = i + "_" + j;
            }
        }
        StartCoroutine(WaitOneFrame());
    }

    public void PauseButton()
    {
        if (Time.timeScale == 0) Time.timeScale = 1;
        else Time.timeScale = 0;
    }

    IEnumerator WaitOneFrame()
    {
        yield return 0;
        NewLevel();
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        newObject = Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
        newObject.GetComponent<RectTransform>().sizeDelta = new Vector2(distanseStep, distanseStep);
        newObject.transform.SetParent(rectGameField.transform.FindChild("PlayerLayer").transform);
        newObject.transform.localScale = new Vector2(1, 1);
        playerControl = newObject.GetComponent<PlayerController>();
        playerControl.gameMan = this;
    }

    private void NewLevel()
    {
        ReloadWaterField();
        ReloadWaterEnemy();
        ReloadGroundEnemy();
        health = 3;
        textProgress.text = "0%";
    }

    private void ReloadWaterField()
    {
        startSumWater = 0;
        for (var i = 1; i <= grid_screen_width; i++)
        {
            for (var j = 1; j <= grid_screen_height; j++)
            {
                if (i <= 2 || i >= grid_screen_width - 1 || j <= 2 || j >= grid_screen_height - 1)
                {
                    GridElement[i, j].GetComponent<Image>().color = colorCell[0];
                    GridElement[i, j].status = "ground";
                }
                else
                {
                    GridElement[i, j].GetComponent<Image>().color = colorCell[1];
                    GridElement[i, j].status = "water";
                    startSumWater++;
                }
            }
        }
        currentSumWater = startSumWater;
    }

    private void ReloadWaterEnemy()
    {
        WaterEnemy = new WaterEnemyController[level / 2 + 1];

        for (var i = 0; i < level / 2 + 1; i++)
        {
            newObject = Instantiate(Resources.Load("WaterEnemy", typeof(GameObject))) as GameObject;
            newObject.transform.SetParent(rectGameField.transform.FindChild("EnemyLayer"));
            newObject.transform.localScale = new Vector2(1, 1);
            WaterEnemy[i] = newObject.GetComponent<WaterEnemyController>();
            WaterEnemy[i].gameMan = this;
            newObject.GetComponent<Image>().color = colorCell[3];
            newObject.GetComponent<RectTransform>().sizeDelta = new Vector2(distanseStep, distanseStep);
            do
            {
                WaterEnemy[i].pos = new[] { Random.Range(6, grid_screen_width - 4), Random.Range(6, grid_screen_height - 4) };
                if (GetCellStatus(WaterEnemy[i].pos[0], WaterEnemy[i].pos[1]) != "water")
                    WaterEnemy[i].pos = new[] { 0, 0 };
                else
                {
                    for (var j = 0; j < i; j++)
                    {
                        if (WaterEnemy[i].pos[0] == WaterEnemy[j].pos[0] && WaterEnemy[i].pos[1] == WaterEnemy[j].pos[1]) WaterEnemy[i].pos = new[] { 0, 0 };
                    }
                }

            }
            while (WaterEnemy[i].pos[0] == 0);
            WaterEnemy[i].GetComponent<RectTransform>().localPosition = GridElement[WaterEnemy[i].pos[0], WaterEnemy[i].pos[1]].gameObject.transform.localPosition;
        }
    }

    private void ReloadGroundEnemy()
    {
        GroundEnemy = new GroundEnemyController[level - level/2];
        for (var i = 0; i < level - level / 2; i++)
        {
            newObject = Instantiate(Resources.Load("GroundEnemy", typeof(GameObject))) as GameObject;
            newObject.transform.SetParent(rectGameField.transform.FindChild("EnemyLayer"));
            newObject.transform.localScale = new Vector2(1, 1);
            GroundEnemy[i] = newObject.GetComponent<GroundEnemyController>();
            GroundEnemy[i].gameMan = this;
            newObject.GetComponent<Image>().color = colorCell[3];
            newObject.GetComponent<RectTransform>().sizeDelta = new Vector2(distanseStep, distanseStep);

            do
            {
                GroundEnemy[i].pos = new[] { Random.Range(1, grid_screen_width + 1), Random.Range(grid_screen_height - 1, grid_screen_height + 1) };
                if (GetCellStatus(GroundEnemy[i].pos[0], GroundEnemy[i].pos[1]) != "ground")
                    GroundEnemy[i].pos = new[] { 0, 0 };
                else
                {
                    for (var j = 0; j < i; j++)
                    {
                        if (GroundEnemy[i].pos[0] == GroundEnemy[j].pos[0] && GroundEnemy[i].pos[0] == GroundEnemy[j].pos[0])
                            GroundEnemy[i].pos = new[] { 0, 0 };
                    }
                }
            }
            while (GroundEnemy[i].pos[0] == 0);
            GroundEnemy[i].GetComponent<RectTransform>().localPosition = GridElement[GroundEnemy[i].pos[0], GroundEnemy[i].pos[1]].gameObject.transform.localPosition;
        }
    }

    public void GameOver()
    {
        for (var i = 0; i < GroundEnemy.Length; i++)
        {
            if (GroundEnemy[i]) Destroy(GroundEnemy[i].gameObject);
        }
        if (health > 1)
        {
            if (playerControl.inWater) FillTrack(playerControl.GetPosition()[0], playerControl.GetPosition()[1]);
            ReloadGroundEnemy();
            health--;
        }
        else
        {
            for (var i = 0; i < WaterEnemy.Length; i++)
            {
                if (WaterEnemy[i]) Destroy(WaterEnemy[i].gameObject);
            }
            level = 1;
            NewLevel();
        }
        leftTime = 60;
        playerControl.StartPosition();
        textHP.text = health + "";
    }

    private void FillTrack(int x, int y)
    {
        if (x > 2 && x < grid_screen_width - 1 && y > 2 && y < grid_screen_height - 1)
        {
            GridElement[x, y].status = "water";
            GridElement[x, y].GetComponent<Image>().color = colorCell[1];
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if (x + i >= 0 && y + j >= 0)
                    {
                        if (GridElement[x + i, y + j] & GetCellStatus(x + i, y + j) == "track") FillTrack(x + i, y + j);
                    }
                }
            }
        }
    }

    public Vector2 GetCellPosition(int x, int y)
    {
        return GridElement[x, y].GetComponent<RectTransform>().localPosition;
    }

    public string GetCellStatus(int x, int y)
    {
        return GridElement[x, y].status;
    }

    public void ChangeColorMove(int x, int y)
    {
        if (GetCellStatus(x, y) == "water")
        {
            GridElement[x, y].GetComponent<Image>().color = colorCell[2];
            GridElement[x, y].status = "track";
        }
    }

    public void FillNewGround()
    {
        biggestEnemyTerritory = 0;
        SumCellInTerritory = new List<int>(1) {0};
        for (var i = 1; i <= grid_screen_width; i++)
        {
            for (var j = 1; j <= grid_screen_height; j++)
            {
                if (GetCellStatus(i, j) == "water")
                {
                    SumCellInTerritory.Add(SumCellInTerritory.Count+ 1);
                    AllTerritory(SumCellInTerritory.Count - 1, i, j);
                }
            }
        }
        currentSumWater = 0;
        for (var i = 1; i <= grid_screen_width; i++)
        {
            for (var j = 1; j <= grid_screen_height; j++)
            {
                if (GetCellStatus(i, j) == "temp" && SumCellInTerritory[GridElement[i, j].territoryIndex] == biggestEnemyTerritory)
                {
                    currentSumWater += 1;
                    GridElement[i, j].status = "water";
                }
                else
                {
                    GridElement[i, j].status = "ground";
                    GridElement[i, j].GetComponent<Image>().color = colorCell[0];
                }
            }
        }
        for (var i = 0; i < WaterEnemy.Length; i++)
        {
            if (WaterEnemy[i])
            {
                if (SumCellInTerritory[GridElement[WaterEnemy[i].pos[0], WaterEnemy[i].pos[1]].territoryIndex] != biggestEnemyTerritory)
                {
                    Destroy(WaterEnemy[i].gameObject);
                    WaterEnemy[i] = null;
                }
            }
        }
        textProgress.text = (int)(100 * (1 - currentSumWater / (float)startSumWater)) + "%";
        if ((int) (100 * (1 - currentSumWater / (float) startSumWater)) >= 75)
        {
            for (var i = 0; i < WaterEnemy.Length; i++)
            {
                if(WaterEnemy[i]) Destroy(WaterEnemy[i].gameObject);
            }
            for (var i = 0; i < GroundEnemy.Length; i++)
            {
                if (GroundEnemy[i]) Destroy(GroundEnemy[i].gameObject);
            }
            level += 1;
            NewLevel();
            leftTime = 60;
            playerControl.StartPosition();
        }
        
    }

    private void AllTerritory(int index, int x, int y)
    {
        GridElement[x, y].status = "temp";
        GridElement[x, y].territoryIndex = index;
        SumCellInTerritory[SumCellInTerritory.Count -1] += 1;
        if (SumCellInTerritory[SumCellInTerritory.Count -1] > biggestEnemyTerritory)
            biggestEnemyTerritory = SumCellInTerritory[SumCellInTerritory.Count -1];
        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if (GetCellStatus(x + i, y + j) == "water")
                {
                    AllTerritory(index, x + i, y + j);
                }
            }
        }
    }

    void Update ()
    {
        leftTime -= Time.deltaTime;
        if(leftTime <= 0) GameOver();
        textTime.text = (int) leftTime + "";

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
