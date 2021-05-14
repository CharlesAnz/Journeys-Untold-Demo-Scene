using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartScreenManager : MonoBehaviour
{
    public void TurnArrow(Image arrow)
    {
        arrow.rectTransform.eulerAngles = new Vector3(
            arrow.rectTransform.eulerAngles.x,
            arrow.rectTransform.eulerAngles.y,
            0);
    }

    public void StartGame(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }



}
