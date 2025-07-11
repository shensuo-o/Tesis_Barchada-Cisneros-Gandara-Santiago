using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSpawn : MonoBehaviour
{
    public static SaveSpawn Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
