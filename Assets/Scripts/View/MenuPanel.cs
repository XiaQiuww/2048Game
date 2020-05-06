using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    public SelectModelPanel selectModelPanel;
    public SetPanel setPanel;

    /// <summary>
    /// 点击 开始游戏 按钮
    /// </summary>
    public void OnStartGameClick()
    {
        selectModelPanel.Show();
    }

    /// <summary>
    /// 点击 设置 按钮
    /// </summary>
    public void OnSetClick()
    {
        setPanel.Show();
    }

    /// <summary>
    /// 点击 退出游戏 按钮
    /// </summary>
    public void OnExitClick()
    {
        Application.Quit();
    }
}
