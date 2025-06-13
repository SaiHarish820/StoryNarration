using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class neversleep : MonoBehaviour
{
    void Start()
    {
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

}
