using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public Text textScore;
    public Text textBestScore;
    public Button btnLast;
    public Button btnReStart;
    public Button btnExit;
    public WinPanel winPanel;
    public LosePanel losePanel;

    public Transform gridParent; // 格子的父物体

    private Dictionary<int, int> gridDict = new Dictionary<int, int>() { { 4, 80 }, { 5, 65 }, { 6, 55 } };

    /// <summary>
    /// 行
    /// </summary>
    private int row;
    /// <summary>
    /// 列
    /// </summary>
    private int col;

    public GameObject gridPrefab;
    public GameObject numberPrefab;

    public MyGrid[][] grids; // 所有的格子

    private List<MyGrid> canCreatNumberGFrid = new List<MyGrid>();

    private Vector3 pointerDownPos, pointerUpPos;

    private bool isNeedCreateNumber = false;

    public int currentScore;

    public StepModel lastStepModel;

    /*生命周期----------------------------------------------------------------------*/
    private void Awake()
    {
        btnLast.interactable = false;
        UpdateBestScore(PlayerPrefs.GetInt(Const.BestScore, 0));
        lastStepModel = new StepModel();
        InitGrid();
        CreateNumber();
    }
    /*End-生命周期------------------------------------------------------------------*/



    /// <summary>
    /// 计算鼠标移动的类型
    /// </summary>
    /// <returns>MoveType</returns>
    public MoveType CaculateMoveType()
    {
        if (Mathf.Abs(pointerUpPos.x - pointerDownPos.x) > Mathf.Abs(pointerUpPos.y - pointerDownPos.y))
        {
            // 左右移动
            if (pointerUpPos.x > pointerDownPos.x)
            {
                // 向右移动
                return MoveType.Right;
            }
            else
            {
                // 向左移动
                return MoveType.Left;
            }
        }
        else
        {
            // 上下移动
            if (pointerUpPos.y > pointerDownPos.y)
            {
                // 向上移动
                return MoveType.Top;
            }
            else
            {
                // 向下移动
                return MoveType.Down;
            }
        }
    }

    public void MoveNumber(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.Top:
                for (int j = 0; j < col; j++)
                {
                    for (int i = 1; i < row; i++)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {
                            Number number = grids[i][j].GetNumber();

                            for (int m = i - 1; m >= 0; m--)
                            {
                                Number targetNum = null;

                                if (grids[m][j].IsHaveNumber())
                                    targetNum = grids[m][j].GetNumber();

                                HandleNumber(number, targetNum, grids[m][j]);

                                if (targetNum != null)
                                    break;
                            }
                        }
                    }
                }
                break;

            case MoveType.Down:
                for (int j = 0; j < col; j++)
                {
                    for (int i = row - 2; i >= 0; i--)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {
                            Number number = grids[i][j].GetNumber();

                            for (int m = i + 1; m < row; m++)
                            {
                                Number targetNum = null;

                                if (grids[m][j].IsHaveNumber())                                
                                    targetNum = grids[m][j].GetNumber();

                                HandleNumber(number, targetNum, grids[m][j]);

                                if (targetNum != null)
                                    break;                                                                
                            }
                        }
                    }
                }
                break;

            case MoveType.Left:
                for (int i = 0; i < row; i++)
                {
                    for (int j = 1; j < col; j++)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {
                            Number number = grids[i][j].GetNumber();

                            for (int m = j - 1; m >= 0; m--)
                            {
                                Number targetNum = null;

                                if (grids[i][m].IsHaveNumber())
                                    targetNum = grids[i][m].GetNumber();

                                HandleNumber(number, targetNum, grids[i][m]);

                                if (targetNum != null)
                                    break;
                            }
                        }
                    }
                }
                break;

            case MoveType.Right:
                for (int i = 0; i < row; i++)
                {
                    for (int j = col - 2; j >=0; j--)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {
                            Number number = grids[i][j].GetNumber();

                            for (int m = j + 1; m < col; m++)
                            {
                                Number targetNum = null;

                                if (grids[i][m].IsHaveNumber())
                                    targetNum = grids[i][m].GetNumber();

                                HandleNumber(number, targetNum, grids[i][m]);

                                if (targetNum != null)
                                    break;
                            }
                        }
                    }
                }
                break;
        }
    }

    public void HandleNumber(Number current,Number target, MyGrid targetGrid)
    {
        if(target != null)
        {
            //判断能不能合并
            if (current.IsMerge(target))
            {
                // 合并
                target.Merge();
                // 销毁当前的这个数字
                current.GetGrid().SetNumber(null);
                //GameObject.Destroy(current.gameObject);
                current.DestroyOnMoveEnd(target.GetGrid());
                isNeedCreateNumber = true;
            }           
        }
        else
        {
            // 没有数字
            current.MoveToGrid(targetGrid);
            isNeedCreateNumber = true;
        }
    }

    public void ResetNumberStatus()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if(grids[i][j].IsHaveNumber())
                {
                    grids[i][j].GetNumber().status = NumberStatus.Normal;
                }
            }
        }
    }


    #region 游戏逻辑
    /// <summary>
    /// 初始化格子（4*4 80 5*5 65 6*6 55
    /// </summary>
    public void InitGrid()
    {
        //获取格子的数量
        int gridNum = PlayerPrefs.GetInt(Const.GameModel, 4);
        GridLayoutGroup gridLayoutGroup = gridParent.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraintCount = gridNum;
        gridLayoutGroup.cellSize = new Vector2(gridDict[gridNum], gridDict[gridNum]);

        grids = new MyGrid[gridNum][];

        //创建格子
        row = gridNum;
        col = gridNum;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grids[i] == null)
                {
                    grids[i] = new MyGrid[gridNum];
                }

                grids[i][j] = CreateGrid();
            }
        }

    }

    /// <summary>
    /// 创建格子
    /// </summary>
    public MyGrid CreateGrid()
    {
        GameObject grid = GameObject.Instantiate(gridPrefab, gridParent);

        return grid.GetComponent<MyGrid>();
    }

    /// <summary>
    /// 创建数字
    /// </summary>
    public void CreateNumber()
    {
        // 找到这个数字所在的格子
        canCreatNumberGFrid.Clear();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grids[i][j].IsHaveNumber() == false)
                {
                    canCreatNumberGFrid.Add(grids[i][j]);
                }
            }
        }

        if (canCreatNumberGFrid.Count == 0)
            return;

        // 随机一个数字
        int index = Random.Range(0, canCreatNumberGFrid.Count);

        // 创建数字 把数字放进去
        GameObject gameObject = GameObject.Instantiate(numberPrefab, canCreatNumberGFrid[index].transform);
        gameObject.GetComponent<Number>().Init(canCreatNumberGFrid[index]);
    }
    public void CreateNumber(MyGrid myGrid, int number)
    {
        GameObject gameObject = GameObject.Instantiate(numberPrefab, myGrid.transform);
        gameObject.GetComponent<Number>().Init(myGrid);
        gameObject.GetComponent<Number>().SetNumber(number);
    }
    #endregion


    #region 游戏流程
    public void RestartGame()
    {
        // 数据清空
        btnLast.interactable = false;
        // 清空分数
        ResetScore();
        // 清空数字
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if(grids[i][j].IsHaveNumber())
                {
                    GameObject.Destroy(grids[i][j].GetNumber().gameObject);
                    grids[i][j].SetNumber(null);
                }                
            }
        }
        // 创建一个数字
        CreateNumber();
    }

    public void GameWin()
    {
        Debug.Log("win");
        winPanel.Show();
    }

    public void GameLose()
    {
        Debug.Log("lose");
        losePanel.Show();
    }

    /// <summary>
    /// 返回上一步
    /// </summary>
    public void BackToLastStep() 
    {
        //分数返回到上一步
        currentScore = lastStepModel.score;
        UpdateScore(lastStepModel.score);

        PlayerPrefs.SetInt(Const.BestScore, lastStepModel.bestScore);
        UpdateBestScore(lastStepModel.bestScore);

        //数字返回到上一步
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (lastStepModel.numbers[i][j]==0)
                {
                    if (grids[i][j].IsHaveNumber())
                    {
                        GameObject.Destroy(grids[i][j].GetNumber().gameObject);
                        grids[i][j].SetNumber(null);
                    }
                }
                else if(lastStepModel.numbers[i][j] != 0)
                {
                    if (grids[i][j].IsHaveNumber())
                    {
                        grids[i][j].GetNumber().SetNumber(lastStepModel.numbers[i][j]);
                    }
                    else
                    {
                        CreateNumber(grids[i][j], lastStepModel.numbers[i][j]);
                    }
                }
            }
        }
    }
    #endregion


    #region 事件
    /// <summary>
    /// 鼠标按下
    /// </summary>
    public void OnPointerDown()
    {
        pointerDownPos = Input.mousePosition;
    }

    /// <summary>
    /// 鼠标抬起
    /// </summary>
    public void OnPointerUp()
    {
        pointerUpPos = Input.mousePosition;

        if (Vector3.Distance(pointerDownPos, pointerUpPos) < 20)
            return;

        lastStepModel.UpdateData(this.currentScore, PlayerPrefs.GetInt(Const.BestScore, 0), grids);
        btnLast.interactable = true;

        MoveType moveType = CaculateMoveType();
        MoveNumber(moveType);

        // 移动后产生新数字
        if (isNeedCreateNumber)
            CreateNumber();

        //移动完成后 把所有数字恢复成 可合并状态
        ResetNumberStatus();
        isNeedCreateNumber = false;

        // 判断游戏是否结束
        if (IsGameLose())
            GameLose();
    }

    /// <summary>
    /// 上一步按钮
    /// </summary>
    public void OnLastClick()
    {
        BackToLastStep();
        btnLast.interactable = false;
    }

    public void OnRestartClick()
    {
        RestartGame();
    }

    public void OnExitClick()
    {
        ExitGame();
    }
    #endregion
    
    public void ExitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }

    // 判断游戏是否失败
    public bool IsGameLose()
    {
        // 判断格子是否满了
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grids[i][j].IsHaveNumber() == false)
                {
                    return false;
                }
            }
        }

        // 判断有没有数字能够合并
        for (int i = 0; i < row; i+=2)
        {
            for (int j = 0; j < col; j++)
            {
                MyGrid top = IsHaveGrid(i - 1, j) ? grids[i - 1][j] : null;
                MyGrid down = IsHaveGrid(i + 1, j) ? grids[i + 1][j] : null; 
                MyGrid left = IsHaveGrid(i, j - 1) ? grids[i][j - 1] : null; 
                MyGrid right = IsHaveGrid(i, j + 1) ? grids[i][j + 1] : null;

                if(top != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(top.GetNumber()))
                    {
                        return false;
                    }
                }

                if (down != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(down.GetNumber()))
                    {
                        return false;
                    }
                }

                if (left != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(left.GetNumber()))
                    {
                        return false;
                    }
                }

                if (right != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(right.GetNumber()))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public bool IsHaveGrid(int i,int j)
    {
        if(i >= 0 && i < row && j>=0 && j < col)
        {
            return true;
        }
        return false;
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScore(currentScore);

        // 判断当前分数是否是最高分数
        if ( currentScore > PlayerPrefs.GetInt(Const.BestScore, 0) )
        {
            PlayerPrefs.SetInt(Const.BestScore, currentScore);
            UpdateBestScore(currentScore);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScore(currentScore);
    }

    public void UpdateScore(int score)
    {
        this.textScore.text = score.ToString();
    }

    public void UpdateBestScore(int bestScore)
    {
        this.textBestScore.text = bestScore.ToString();
    }
    /*----------------------------------------------------------------------------*/
}
