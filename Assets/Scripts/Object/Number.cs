using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    private Image bg;
    private Text numberText;

    private MyGrid inGrid; // 这个数字所在的格子

    public NumberStatus status;

    private float spawnScaleTime = 1f;
    private bool isPlayingSpawnAnim = false;

    private float mergeScaleTime = 1f;
    private float mergeScaleTimeBack = 1f;
    private bool isPlayingMergeAnim = false;

    private float movePosTime = 1f;
    private bool isMoving = false;
    private bool isDestroyOnMoveEnd = false;
    private Vector3 startMovePos;

    /*生命周期----------------------------------------------------------------------*/
    private void Awake()
    {
        bg = GetComponent<Image>();
        numberText = transform.Find("Text").GetComponent<Text>();
    }

    private void Update()
    {
        // 数字生成动画
        if (isPlayingSpawnAnim)
        {            
            if (spawnScaleTime <= 1)
            {
                spawnScaleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, spawnScaleTime);
            }
            else
            {
                isPlayingSpawnAnim = false;
            }
        }

        // 数字合并 动画
        if (isPlayingMergeAnim)
        {
            // 数字合并 变大动画
            if (mergeScaleTime <= 1 && mergeScaleTimeBack == 0)
            {
                mergeScaleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, mergeScaleTime);
            }

            // 数字合并 变小动画
            if (mergeScaleTimeBack <= 1 && mergeScaleTime >= 1)
            {
                mergeScaleTimeBack += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, mergeScaleTimeBack);
            }

            if (mergeScaleTime >= 1 && mergeScaleTimeBack >= 1)
            {
                isPlayingMergeAnim = false;
            }
        }    
        
        // 数字移动 动画
        if(isMoving)
        {
            movePosTime += Time.deltaTime * 4;

            transform.localPosition = Vector3.Lerp(startMovePos, Vector3.zero, movePosTime);

            if (movePosTime >= 1)
            {
                isMoving = false;
                if(isDestroyOnMoveEnd)
                {
                    GameObject.Destroy(gameObject);
                }
            } 
        }
    }
    /*End-生命周期------------------------------------------------------------------*/


    public void Init(MyGrid myGrid)
    {
        myGrid.SetNumber(this);
        // 设置所在的格子
        this.SetGrid(myGrid);
        // 给它一个初始化的数字
        this.SetNumber(2);

        status = NumberStatus.Normal;

        PlaySpawnAnim();
    }

    public void SetGrid(MyGrid myGrid)
    {
        this.inGrid = myGrid;
    }

    public MyGrid GetGrid()
    {
        return this.inGrid;
    }

    public void SetNumber(int number)
    {
        this.numberText.text = number.ToString();
    }

    public int GetNumber() // 改成 getnumbertext
    {
        return int.Parse(numberText.text);
    }

    /// <summary>
    /// 把数字移动到某个格子
    /// </summary>
    public void MoveToGrid(MyGrid myGrid)
    {
        transform.SetParent(myGrid.transform);
        //transform.localPosition = Vector3.zero;
        startMovePos = this.transform.localPosition;
        //endMovePos = myGrid.transform.position;
        movePosTime = 0;
        isMoving = true;

        this.GetGrid().SetNumber(null);

        myGrid.SetNumber(this);
        this.SetGrid(myGrid);
    }

    /// <summary>
    /// 合并
    /// </summary>
    public void Merge()
    {
        GamePanel gamePanel = GameObject.Find("Canvas/GamePanel").GetComponent<GamePanel>();

        gamePanel.AddScore(this.GetNumber());

        int number = this.GetNumber() * 2;
        this.SetNumber(number);
        if(number == 2048)
        {
            // 游戏胜利
            gamePanel.GameWin();
        }
        status = NumberStatus.NotMerge;

        PlayMergeAnim();
    }

    /// <summary>
    /// 判断能不能合并
    /// </summary>
    public bool IsMerge(Number number)
    {
        if(this.GetNumber() == number.GetNumber() && number.status == NumberStatus.Normal)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 播放数字生成的动画
    /// </summary>
    public void PlaySpawnAnim()
    {
        spawnScaleTime = 0f;
        isPlayingSpawnAnim = true;
    }

    /// <summary>
    /// 播放数字合并的动画
    /// </summary>
    public void PlayMergeAnim()
    {
        mergeScaleTime = 0;
        mergeScaleTimeBack = 0;
        isPlayingMergeAnim = true;
    }

    public void DestroyOnMoveEnd(MyGrid myGrid)
    {
        transform.SetParent(myGrid.transform);

        startMovePos = this.transform.localPosition;

        movePosTime = 0;
        isMoving = true;

        isDestroyOnMoveEnd = true;
    }

/*----------------------------------------------------------------------------*/
}
