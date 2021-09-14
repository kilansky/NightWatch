using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCone : MonoBehaviour
{

    public GameObject Guard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Thief")
        {
            print("Detect Thief");
            Guard.GetComponent<GuardPathfinding>().Thief = other.gameObject;
            Guard.GetComponent<GuardPathfinding>().ThiefSpotted = true;
        }
    }
}
