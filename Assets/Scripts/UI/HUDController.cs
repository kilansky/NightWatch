using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : SingletonPattern<HUDController>
{
    private GameObject heldObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CameraButton()
    {
        //Toggle the current placement mode of the button
        if (!SecurityPlacement.Instance.placementMode)
        {
            SecurityPlacement.Instance.placementMode = true;

            //Spawn new camera to try and place
            heldObject = Instantiate(SecurityPlacement.Instance.cctvPrefab, Vector3.zero, Quaternion.identity);
            SecurityPlacement.Instance.targetTransform = heldObject.transform; 
        }
        else
        {
            SecurityPlacement.Instance.placementMode = false;

            //Remove held object
            if (heldObject != null)
                Destroy(heldObject);
        }

    }

    public void ShowPauseScreen()
    {

    }

    public void HidePauseScreen()
    {

    }
}
