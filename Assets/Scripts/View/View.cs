using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    /// <summary>
    /// 显示此面板
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏此面板
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }


}
