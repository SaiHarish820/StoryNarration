using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class NavigationButtons : MonoBehaviour
{
    public static void MF_LevelButton()
    {
        SceneManager.LoadScene(1);
    }

    public static void SB_RestartButton1()
    {
        SceneManager.LoadScene(2);
    }

    public static void MF_RestartButton2()
    {
        SceneManager.LoadScene(3);
    }

    public static void MF_RestartButton3()
    {
        SceneManager.LoadScene(4);
    }

    public static void SB_RestartButton4()
    {
        SceneManager.LoadScene(5);
    }

    public static void MF_RestartButton5()
    {
        SceneManager.LoadScene(6);
    }

    public static void MF_RestartButton6()
    {
        SceneManager.LoadScene(7);
    }

    public static void SB_RestartButton7()
    {
        SceneManager.LoadScene(8);
    }




    public static void SB_LevelButton()
    {
        SceneManager.LoadScene(1);
    }






    public GameObject tutorialPopup;
    public GameObject closeArea;    // Assign the full-screen close area


    private Vector3 originalScale;

    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;


    void Start()
    {
        if (tutorialPopup != null)
        {
            originalScale = tutorialPopup.transform.localScale;
            tutorialPopup.SetActive(false);
        }

        if (closeArea != null)
            closeArea.SetActive(false); // Make sure closeArea is hidden on start

        // Check if tutorial was shown before
        if (!PlayerPrefs.HasKey("TutorialShown"))
        {
            ShowTutorial();
            PlayerPrefs.SetInt("TutorialShown", 1);
            PlayerPrefs.Save();
        }
    }

    public void ShowTutorial()
    {
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(true);
            tutorialPopup.transform.localScale = Vector3.zero;

            

            if (closeArea != null)
                closeArea.SetActive(true); // Show close area

            if (audioSource != null && openSound != null)
                audioSource.PlayOneShot(openSound);

            LeanTween.scale(tutorialPopup, originalScale, 0.5f)
                     .setEaseOutBack();
        }
    }

    public void CloseTutorial()
    {
        if (tutorialPopup != null)
        {
            if (audioSource != null && closeSound != null)
                audioSource.PlayOneShot(closeSound);

            LeanTween.scale(tutorialPopup, Vector3.zero, 0.3f)
                     .setEaseInBack()
                     .setOnComplete(() =>
                     {
                         tutorialPopup.SetActive(false);
                         tutorialPopup.transform.localScale = originalScale;

                         

                         if (closeArea != null)
                             closeArea.SetActive(false); // Hide close area
                     });
        }
    }



}
