using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDoor : MonoBehaviour
{
    public GameObject FakeDoorModel;
    public void FakeDoorOff()
    {
        FakeDoorModel.SetActive(false);
    }
    public void FakeDoorOn()
    {
        FakeDoorModel.SetActive(true);
    }
}
