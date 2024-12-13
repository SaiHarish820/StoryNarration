using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private void Awake()
    {
        // Check if an instance already exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        // Set this instance as the singleton instance
        instance = this;

        // Make this GameObject persist across scenes
        DontDestroyOnLoad(gameObject);
    }
}
