using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStateManager : MonoBehaviour
{


    public void HomepageStartButton()
    {
        SceneManager.LoadScene("Level Page");
    }

    public void HomePageQuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
        //Just to make sure its working
    }



}
