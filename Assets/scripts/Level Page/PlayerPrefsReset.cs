using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsReset : MonoBehaviour
{
    public Button resetButton; // Assign this button in the Inspector

    void Start()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ClearAllPlayerPrefs);
        }
    }

    private void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll(); // Clears all saved data
        PlayerPrefs.Save(); // Ensure changes are applied

        Debug.Log("All PlayerPrefs data has been cleared!");
    }
}
