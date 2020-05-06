using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : View
{
    public Slider soundSlider;
    public Slider musicSlider;

    public void OnCloseClick()
    {
        Hide();
    }

    /// <summary>
    /// 音效
    /// </summary>
    /// <param name="f"></param>
    public void OnSoundValueChange(float f)
    {
        PlayerPrefs.SetFloat(Const.Sound, f);
    }

    /// <summary>
    /// 音乐
    /// </summary>
    /// <param name="f"></param>
    public void OnMusicValueChange(float f)
    {
        PlayerPrefs.SetFloat(Const.Music, f);
    }

    public override void Show()
    {
        base.Show();
        //界面初始化
        soundSlider.value = PlayerPrefs.GetFloat(Const.Sound, 0);
        musicSlider.value = PlayerPrefs.GetFloat(Const.Music, 0);
    }
}
