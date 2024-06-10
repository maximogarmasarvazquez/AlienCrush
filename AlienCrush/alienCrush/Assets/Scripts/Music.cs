using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("MusicManager Awake called");
        DontDestroyOnLoad(gameObject);

        // Check if there is another instance of MusicManager
        if (FindObjectsOfType<Music>().Length > 1)
        {
            Destroy(gameObject);
            Debug.Log("Duplicate Music destroyed");
        }
    }
}
