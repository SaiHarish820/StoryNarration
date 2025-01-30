using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NavigationButtons : MonoBehaviour
{
    public static void MF_LevelButton()
    {
        SceneManager.LoadScene(1);
    }

    public static void MF_RestartButton()
    {
        SceneManager.LoadScene(3);
    }

    public static void SB_RestartButton()
    {
        SceneManager.LoadScene(2);
    }

    public static void SB_LevelButton()
    {
        SceneManager.LoadScene(1);
    }


}
