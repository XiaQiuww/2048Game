using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : View
{
    public void OnRestartClick()
    {
        GameObject.Find("Canvas/GamePanel").GetComponent<GamePanel>().RestartGame();
        Hide();
    }

    public void OnExitClick()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
