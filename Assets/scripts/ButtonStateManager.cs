using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStateManager : MonoBehaviour
{

    public GameObject Level1;
    public GameObject Level2;
    public GameObject Level3;
    public GameObject Level4;
    public GameObject Level5;
    public GameObject Level6;

    public void HomepageStartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void HomePageQuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
        //Just to make sure its working
    }

    public void LevelCard1() {

        // Check if the GameObject reference is assigned
        if (Level1 != null)
        {
            // Activate the other GameObject
            Level1.SetActive(true);
        }
        else
        {
            Debug.LogError("Other GameObject is not assigned in the Inspector.");
        }

    }

    public void LevelCard2()
    {

        // Check if the GameObject reference is assigned
        if (Level2 != null)
        {
            // Activate the other GameObject
            Level2.SetActive(true);
        }
        else
        {
            Debug.LogError("Other GameObject is not assigned in the Inspector.");
        }

    }

    public void LevelCard3()
    {

        // Check if the GameObject reference is assigned
        if (Level3 != null)
        {
            // Activate the other GameObject
            Level3.SetActive(true);
        }
        else
        {
            Debug.LogError("Other GameObject is not assigned in the Inspector.");
        }

    }

    public void LevelCard4()
    {

        // Check if the GameObject reference is assigned
        if (Level4 != null)
        {
            // Activate the other GameObject
            Level4.SetActive(true);
        }
        else
        {
            Debug.LogError("Other GameObject is not assigned in the Inspector.");
        }

    }

    public void LevelCard5()
    {

        // Check if the GameObject reference is assigned
        if (Level5 != null)
        {
            // Activate the other GameObject
            Level5.SetActive(true);
        }
        else
        {
            Debug.LogError("Other GameObject is not assigned in the Inspector.");
        }

    }

    public void LevelCard6()
    {

        // Check if the GameObject reference is assigned
        if (Level6 != null)
        {
            // Activate the other GameObject
            Level6.SetActive(true);
        }
        else
        {
            Debug.LogError("Other GameObject is not assigned in the Inspector.");
        }

    }


}
