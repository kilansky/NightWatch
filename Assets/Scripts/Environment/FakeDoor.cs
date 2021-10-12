using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDoor : MonoBehaviour
{
    public MeshRenderer FakeDoorModel;
    public MeshRenderer DoorModel;
    public float VisibleDuration;
    private float timeLeft = 0;
      
    public void FakeDoorOff()
    {
        //print("Set Fake Door To Off");
        FakeDoorModel.enabled = false;
        DoorModel.enabled = true;
        if (timeLeft <= 0)
        {
            timeLeft = VisibleDuration;
            StartCoroutine(DoorVisible());
        }
        timeLeft = VisibleDuration;

    }
    public void FakeDoorOn()
    {
        FakeDoorModel.enabled = true;
        DoorModel.enabled = false;
    }

    private IEnumerator DoorVisible()
    {
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        
        FakeDoorOn();
    }
}
