using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class LevelManager_Miracle : MonoBehaviour
{
    public static LevelManager_Miracle instance;

    [SerializeField] private float timeLimit = 0;
    [SerializeField] private int maxHiddenObjectToFound = 0;
    [SerializeField] private ObjectHolder objectHolderPrefab;

    [HideInInspector] public GameStatus gameStatus = GameStatus.NEXT;
    private List<HiddenObjectData> activeHiddenObjectList;
    private float currentTime;
    private int totalHiddenObjectsFound = 0;
    private TimeSpan time;
    private RaycastHit2D hit;
    private Vector3 pos;

    private int starsEarned = 0;  // Stores the number of stars earned
    private string levelKey; // Unique key for saving progress


    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip gameWinSound;
    [SerializeField] private ParticleSystem gameOverParticles;
    [SerializeField] private ParticleSystem gameWinParticles;

    [SerializeField] private Transform gameOverParticlePosition;
    [SerializeField] private Transform gameWinParticlePosition;

    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip popSound;

    [SerializeField][Range(0f, 1f)] private float loweredVolumeOnPopup = 0.2f;



    private AudioSource audioSource;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        levelKey = "Level" + SceneManager.GetActiveScene().buildIndex + "_Stars";
        Debug.Log("Level Key: " + levelKey);

        audioSource = GetComponent<AudioSource>(); // Add AudioSource to the same GameObject
        activeHiddenObjectList = new List<HiddenObjectData>();
        AssignHiddenObjects();
    }

    void AssignHiddenObjects()
    {
        ObjectHolder objectHolder = Instantiate(objectHolderPrefab, Vector3.zero, Quaternion.identity);
        totalHiddenObjectsFound = 0;
        activeHiddenObjectList.Clear();
        gameStatus = GameStatus.PLAYING;
        UIManager.instance.TimerText.text = "" + timeLimit;
        currentTime = timeLimit;

        for (int i = 0; i < objectHolder.HiddenObjectList.Count; i++)
        {
            objectHolder.HiddenObjectList[i].hiddenObj.GetComponent<Collider2D>().enabled = false;
        }

        int k = 0;
        while (k < maxHiddenObjectToFound)
        {
            int randomNo = UnityEngine.Random.Range(0, objectHolder.HiddenObjectList.Count);
            if (!objectHolder.HiddenObjectList[randomNo].makeHidden)
            {
                objectHolder.HiddenObjectList[randomNo].hiddenObj.name = "" + k;
                objectHolder.HiddenObjectList[randomNo].makeHidden = true;
                objectHolder.HiddenObjectList[randomNo].hiddenObj.GetComponent<Collider2D>().enabled = true;
                activeHiddenObjectList.Add(objectHolder.HiddenObjectList[randomNo]);
                k++;
            }
        }

        UIManager.instance.PopulateHiddenObjectIcons(activeHiddenObjectList);
        gameStatus = GameStatus.PLAYING;
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleObjectClick();
            }

            currentTime -= Time.deltaTime;
            time = TimeSpan.FromSeconds(currentTime);
            UIManager.instance.TimerText.text = time.ToString(@"mm\:ss");

            if (currentTime <= 0)
            {
                HandleGameOver();
            }
        }
    }

    void HandleObjectClick()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit && hit.collider != null)
        {
            hit.collider.gameObject.SetActive(false);
            UIManager.instance.CheckSelectedHiddenObject(hit.collider.gameObject.name);

            for (int i = 0; i < activeHiddenObjectList.Count; i++)
            {
                if (activeHiddenObjectList[i].hiddenObj.name == hit.collider.gameObject.name)
                {

                    //  Play pop sound effect
                    if (audioSource && popSound)
                        audioSource.PlayOneShot(popSound);

                    activeHiddenObjectList.RemoveAt(i);
                    break;
                }
            }

            totalHiddenObjectsFound++;

            if (totalHiddenObjectsFound >= maxHiddenObjectToFound)
            {
                HandleGameWin();
            }
        }
    }

    void HandleGameOver()
    {
        if (gameStatus == GameStatus.NEXT) return;

        Debug.Log("Time Up - Game Over!");
        UIManager.instance.AnimatePopup(UIManager.instance.GameOverObj);
        UIManager.instance.HideAllHiddenObjectIcons();

        if (backgroundMusic)
            backgroundMusic.volume = loweredVolumeOnPopup;

        if (audioSource && gameOverSound)
            audioSource.PlayOneShot(gameOverSound);

        if (gameOverParticles && gameOverParticlePosition)
            Instantiate(gameOverParticles, gameOverParticlePosition.position, Quaternion.identity);

        gameStatus = GameStatus.NEXT;
        starsEarned = 0;
    }


    void HandleGameWin()
    {
        if (gameStatus == GameStatus.NEXT) return;

        Debug.Log("You won the game!");
        UIManager.instance.AnimatePopup(UIManager.instance.GameCompleteObj);

        if (backgroundMusic)
            backgroundMusic.volume = loweredVolumeOnPopup;

        if (audioSource && gameWinSound)
            audioSource.PlayOneShot(gameWinSound);

        if (gameWinParticles && gameWinParticlePosition)
            Instantiate(gameWinParticles, gameWinParticlePosition.position, Quaternion.identity);

        CalculateStars();
        gameStatus = GameStatus.NEXT;
    }

    



    public void NextButton()
    {
        if (LevelManager_Miracle.instance.backgroundMusic)
            LevelManager_Miracle.instance.backgroundMusic.volume = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    void CalculateStars()
    {
        float timeRemainingPercentage = currentTime / timeLimit;

        if (timeRemainingPercentage > 0.66f)
        {
            starsEarned = 3;
        }
        else if (timeRemainingPercentage > 0.33f)
        {
            starsEarned = 2;
        }
        else
        {
            starsEarned = 1;
        }

        Debug.Log("Stars Earned: " + starsEarned);
        //UIManager.instance.UpdateStarDisplay(starsEarned); 

        // Save highest star count
        int savedStars = PlayerPrefs.GetInt(levelKey, 0);
        if (starsEarned > savedStars)
        {
            PlayerPrefs.SetInt(levelKey, starsEarned);
            PlayerPrefs.Save();
        }
    }

    public IEnumerator HintObject()
    {
        if (activeHiddenObjectList.Count > 0)
        {
            int randomValue = UnityEngine.Random.Range(0, activeHiddenObjectList.Count);
            Vector3 originalScale = activeHiddenObjectList[randomValue].hiddenObj.transform.localScale;
            activeHiddenObjectList[randomValue].hiddenObj.transform.localScale = originalScale * 1.25f;
            yield return new WaitForSeconds(0.25f);
            activeHiddenObjectList[randomValue].hiddenObj.transform.localScale = originalScale;
        }
    }
}

[System.Serializable]
public class HiddenObjectData
{
    public string name;
    public GameObject hiddenObj;
    public bool makeHidden = false;
}

public enum GameStatus
{
    PLAYING,
    NEXT
}