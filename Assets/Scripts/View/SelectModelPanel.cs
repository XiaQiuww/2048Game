using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectModelPanel : View
{
    /// <summary>
    /// 点击 选择模式 按钮
    /// </summary>
    /// <param name="count"></param>
    public void OnSelectModelClick(int count)
    {
        //选择模式
        PlayerPrefs.SetInt(Const.GameModel, count);
        //跳转场景 到 游戏场景
        SceneManager.LoadSceneAsync(1);        
    }
}
