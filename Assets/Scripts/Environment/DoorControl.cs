using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public float animationDuration;
    private Animator myAnimator;

    private bool IsOpenable;
    private bool IsCloseable;

    private void Awake()
    {
        IsOpenable = true;
        IsCloseable = true;
        myAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        AnimationClip[] clips = myAnimator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip c in clips)
        {
            if(c.name == "Open")
            {
                animationDuration = c.length;
            }
        }
    }

    
    public void OpenDoor()
    {
        if (IsOpenable)
        {
            myAnimator.SetTrigger("OpenDoor");
            print("Door Opens");
        }
        
    }

    public void CloseDoor()
    {
        if(IsCloseable)
        {
            myAnimator.SetTrigger("CloseDoor");
            print("Door Closes");
            CloseDoorCoroutine();
        }
    }

    private IEnumerator CloseDoorCoroutine()
    {
        yield return new WaitForSeconds(2f);
    }
}
