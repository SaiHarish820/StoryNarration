using UnityEngine;
using UnityEngine.SceneManagement; // Required for accessing scene information

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private void Update()
    {
        // Check if an instance already exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        // Set this instance as the singleton instance
        instance = this;

        // Check current scene name
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "HomePage" && sceneName != "Level Page")
        {
            
            Destroy(gameObject); // Destroy this GameObject if it's not in the specified scenes
            return;
        }

        // Make this GameObject persist across scenes
        DontDestroyOnLoad(gameObject);
    }
}
