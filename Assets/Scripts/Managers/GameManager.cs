using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonPattern<GameManager>
{
    public GameObject sceneViewMask;

    // Start is called before the first frame update
    void Start()
    {
        sceneViewMask.SetActive(false);
    }
}
