using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    // Make the method public so it can be accessed from UI buttons
    public void BackToLevelPage()
    {
        SceneManager.LoadScene("Level Page");
    }

    // If you want the player to press a key to go back
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Example: Press Escape key to go back
        {
            BackToLevelPage();
        }
    }
}
