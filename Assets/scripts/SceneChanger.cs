using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadHomePageAfterDelay());
    }

    IEnumerator LoadHomePageAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("HomePage");
    }
}
