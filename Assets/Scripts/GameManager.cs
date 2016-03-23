using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum GameState
{
    Playing,
    GameOver,
    WaitingForMoveToEnd
}

public class GameManager : MonoBehaviour
{
    // 延迟相关
    public GameState State;
    [Range(0, 2f)] public float delay;  
    private bool moveMade;
    private bool[] lineMoveComplete = new[] {true, true, true, true};


    public GameObject YouWonText;
    public GameObject GameOverText;
    public Text GameOverScoreText;
    public GameObject GameOverPanel;

    private Tile[,] AllTiles = new Tile[4,4];
    private List<Tile[]>  columns = new List<Tile[]>();
    private List<Tile[]> rows = new List<Tile[]>();
    private List<Tile> EmptyTiles = new List<Tile>();

    void Start()
    {
        Tile[] AllTilesOneDim = GameObject.FindObjectsOfType<Tile>();
        for (int i = 0; i < AllTilesOneDim.Length; i++)
        {
            AllTilesOneDim[i].Number = 0;
            AllTiles[AllTilesOneDim[i].indRow,AllTilesOneDim[i].indCol] = AllTilesOneDim[i];
        }

        //Generate();     // 这样单个弄不太好
        //Generate();
        for (int i = 0; i < 2; i++)   // 这个2 也应该抽象出一个变量
        {
            Generate();
        }
    }

    public void Move(MoveDirection md)
    {
        Debug.Log(md.ToString() + " move.");
        ResetMergedFlags();

        bool moveMade = false;   // 玩家这次操作  是否发生移动
        if (delay > 0)
        {
            StartCoroutine(MoveCoroutine(md));
        }
        else
        {
            for (int i = 0; i < rows.Count; i++)
            {
                switch (md)
                {
                    case MoveDirection.Down:
                        while (MakeOneMoveUpIndex(columns[i]))
                        {
                            moveMade = true;
                        }
                        break;
                    case MoveDirection.Left:
                        while (MakeOneMoveDownIndex(rows[i]))
                        {
                            moveMade = true;
                        }
                        break;
                    case MoveDirection.Right:
                        while (MakeOneMoveUpIndex(rows[i]))
                        {
                            moveMade = true;
                        }
                        break;
                    case MoveDirection.Up:
                        while (MakeOneMoveDownIndex(columns[i]))
                        {
                            moveMade = true;
                        }
                        break;
                    default:
                        Debug.LogError("未经处理的类型");
                        break;
                }
            }
            if (moveMade)
            {
                UpdateEmptyTiles();
                Generate();

                if (!CanMove())
                {
                    GameOver();
                }
            }
        }
    }

    IEnumerator MoveCoroutine(MoveDirection md)
    {
        State = GameState.WaitingForMoveToEnd;
        switch (md)
        {
            case MoveDirection.Down:
                for (int i = 0; i < columns.Count; i++)
                {
                    StartCoroutine(MoveOneLineUpIndexCoroutine(columns[i], i));
                }
                break;
            case MoveDirection.Left:
                for (int i = 0; i < rows.Count; i++)
                {
                    StartCoroutine(MoveOneLineDownIndexCoroutine(rows[i], i));
                }
                break;
            case MoveDirection.Right:
                for (int i = 0; i < rows.Count; i++)
                {
                    StartCoroutine(MoveOneLineUpIndexCoroutine(rows[i], i));
                }
                break;
            case MoveDirection.Up:
                for (int i = 0; i < columns.Count; i++)
                {
                    StartCoroutine(MoveOneLineDownIndexCoroutine(columns[i], i));
                }
                break;
        }
        // 等到 所有行都移动完成
        while (!(lineMoveComplete[0] && lineMoveComplete[1] && lineMoveComplete[2] && lineMoveComplete[3]))
        {
            yield return null;
        }

        if (moveMade)
        {
            UpdateEmptyTiles();
            Generate();
            if (!CanMove())
            {
                GameOver();
            }
            else
            {
                State = GameState.Playing;
            }
        }
        StopAllCoroutines();
    }

    IEnumerator MoveOneLineUpIndexCoroutine(Tile[] line, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveUpIndex(line))
        {
            moveMade = true;
            yield return new WaitForSeconds(delay);
        }
        lineMoveComplete[index] = true;
    }

    IEnumerator MoveOneLineDownIndexCoroutine(Tile[] line, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveDownIndex(line))
        {
            moveMade = true;
            yield return new WaitForSeconds(delay);
        }
        lineMoveComplete[index] = true;
    }

    private void UpdateEmptyTiles()
    {
        EmptyTiles.Clear();
        foreach (Tile tile in AllTiles)
        {
            if (tile.Number == 0)
            {
                EmptyTiles.Add(tile);
            }
        }
    }

    // 将列表中 从小头开始，索引大的 向 索引小的  移动（前移）注意只是移动一步
    bool MakeOneMoveDownIndex(Tile[] LineOffTiles)
    {
        for (int i = 0; i < LineOffTiles.Length - 1; i++)
        {
            // 移动 块！
            if (LineOffTiles[i].Number == 0 && LineOffTiles[i + 1].Number != 0)
            {
                LineOffTiles[i].Number = LineOffTiles[i + 1].Number;
                LineOffTiles[i + 1].Number = 0;
                return true;
            }
            
            // 合并 块！
            if (LineOffTiles[i].Number !=0 && LineOffTiles[i].Number == LineOffTiles[i+1].Number && 
                LineOffTiles[i].mergedThisTurn ==false
                && LineOffTiles[i+1].mergedThisTurn == false)
            {
                LineOffTiles[i].Number *= 2;
                LineOffTiles[i + 1].Number = 0;
                LineOffTiles[i].mergedThisTurn = true;
                LineOffTiles[i].PlayMergedAnimation();
                ScoreTracker.Instance.Score += LineOffTiles[i].Number;

                if (LineOffTiles[i].Number == 2048)
                {
                    YouWon();
                }
                return true;
            }
        }
        return false;
    }

    // 将列表中从大头开始， 索引小的 向 索引大的  移动（后移） 注意只是移动一步
    bool MakeOneMoveUpIndex(Tile[] LineOffTiles)
    {
        for (int i = LineOffTiles.Length - 1; i > 0; i--)
        {
            // 移动块！
            if (LineOffTiles[i].Number == 0 && LineOffTiles[i - 1].Number != 0)
            {
                LineOffTiles[i].Number = LineOffTiles[i - 1].Number;
                LineOffTiles[i - 1].Number = 0;
                return true;
            }

            // 合并 块！
            if (LineOffTiles[i].Number != 0 && LineOffTiles[i].Number == LineOffTiles[i - 1].Number &&
                LineOffTiles[i].mergedThisTurn == false
                && LineOffTiles[i - 1].mergedThisTurn == false)
            {
                LineOffTiles[i].Number *= 2;
                LineOffTiles[i - 1].Number = 0;
                LineOffTiles[i].mergedThisTurn = true;
                LineOffTiles[i].PlayMergedAnimation();
                ScoreTracker.Instance.Score += LineOffTiles[i].Number;
                if (LineOffTiles[i].Number == 2048)
                {
                    YouWon();
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 重置 回合中 合并的标识
    /// </summary>
    private void ResetMergedFlags()
    {
        foreach (var tile in AllTiles)
        {
            tile.mergedThisTurn = false;
        }
    }

    private void GameOver()
    {
        GameOverText.SetActive(true);
        GameOverScoreText.text = ScoreTracker.Instance.Score.ToString();
        GameOverPanel.SetActive(true);
        YouWonText.SetActive(false);
    }

    private void YouWon()
    {
        GameOverText.SetActive(false);
        YouWonText.SetActive(true);
        GameOverScoreText.text = ScoreTracker.Instance.Score.ToString();
        GameOverPanel.SetActive(true);
    }

    bool CanMove()
    {
        if (EmptyTiles.Count > 0)
        {
            return true;
        }
        else
        {
            // 检查 列
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < rows.Count - 1; j++)
                {
                    if (AllTiles[j, i].Number == AllTiles[j+1, i].Number)
                    {
                        return true;
                    }
                }
            }
            // 检查 行
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < rows.Count - 1; j++)
                {
                    if (AllTiles[i, j].Number == AllTiles[i, j+1].Number)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //void Update()
    //{
    //    // 临时测试生成 
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        Generate();
    //    }
    //}

    // 在空的位置生成2 
    void Generate()
    {
        if (EmptyTiles.Count > 0)
        {
            int indexForNewNumber = Random.Range(0, EmptyTiles.Count);
            int randomNum = Random.Range(0, 10);
            if (randomNum == 0)
            {
                EmptyTiles[indexForNewNumber].Number = 4;
            }
            else
            {
                EmptyTiles[indexForNewNumber].Number = 2;
            }
            
            EmptyTiles[indexForNewNumber].PlayAppearAnimation();
            
            EmptyTiles.RemoveAt(indexForNewNumber);
        }
    }

    public void NewGameButtonHandler()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
} 
