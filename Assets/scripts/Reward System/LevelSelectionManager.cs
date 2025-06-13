using UnityEngine;

public class LevelSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelStarGroup
    {
        public GameObject EmptyStars_Slot; // No stars earned
        public GameObject One_Star;       // 1-star rating
        public GameObject Two_Stars;      // 2-star rating
        public GameObject Three_Stars;    // 3-star rating
    }

    public LevelStarGroup[] levelStarGroups; // Array to hold star groups for each level

    void Start()
    {
        int totalLevels = levelStarGroups.Length;

        for (int i = 0; i < totalLevels; i++)
        {
            int levelNumber = i + 2; // Adjusting for Build Index starting from 2
            string levelKey = "Level" + levelNumber + "_Stars";
            int stars = PlayerPrefs.GetInt(levelKey, 0);
            Debug.Log("Loading Stars for: " + levelKey + " = " + stars);

            // Update star display for the level
            UpdateStarDisplay(levelStarGroups[i], stars);
        }
    }

    void UpdateStarDisplay(LevelStarGroup starGroup, int stars)
    {
        // Enable the correct star display and disable others
        starGroup.EmptyStars_Slot.SetActive(stars == 0);
        starGroup.One_Star.SetActive(stars == 1);
        starGroup.Two_Stars.SetActive(stars == 2);
        starGroup.Three_Stars.SetActive(stars == 3);
    }
}
