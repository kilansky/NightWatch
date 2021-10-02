using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public float openAnimationDuration;
    public float closeAnimationDuration;
    public float chaseOpenDuration;
    public float chaseCloseDuration;
    public GameObject DoorModel;

    [HideInInspector]  public bool IsOpened;
    [HideInInspector]  public bool IsClosed;
    [HideInInspector] public Transform ExitPosition;

    [SerializeField] private Transform frontDoorPosition;
    [SerializeField] private Transform backDoorPosition;
    private Animator myAnimator;

    private void Awake()
    {
        IsOpened = false;
        IsClosed = true;
        myAnimator = DoorModel.GetComponent<Animator>();
        
    }

    private void Start()
    {
        
        AnimationClip[] clips = myAnimator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip c in clips)
        {
            if(c.name == "Open")
            {
                openAnimationDuration = c.length;
            }
        }
    }

    
    public void OpenDoor()
    {
        if (IsClosed)
        {
            myAnimator.SetFloat("BaseSpeed", (1 / openAnimationDuration));
            myAnimator.SetTrigger("OpenDoor");
            IsOpened = true;
            IsClosed = false;
            print("Door Opens");
        }
        
    }

    public void CloseDoor()
    {
        if(IsOpened)
        {
            myAnimator.SetFloat("BaseSpeed", (1 / closeAnimationDuration));
            myAnimator.SetTrigger("CloseDoor");
            IsOpened = false;
            IsClosed = true;
            print("Door Closes");
        }
    }

    public void ChaseOpenDoor()
    {
        if (IsClosed)
        {
            myAnimator.SetFloat("BaseSpeed", (1 / chaseOpenDuration));
            myAnimator.SetTrigger("OpenDoor");
            IsOpened = true;
            IsClosed = false;
            print("Door Opens");
        }

    }

    public void ChaseCloseDoor()
    {
        if (IsOpened)
        {
            myAnimator.SetFloat("BaseSpeed", (1 / chaseCloseDuration));
            myAnimator.SetTrigger("CloseDoor");
            IsOpened = false;
            IsClosed = true;
            print("Door Closes");
        }
    }

    public Vector3 GetWaitPosition(Vector3 position)
    {
        Vector3 targetDirection = transform.position - position;

        float directionAngle = Vector3.Angle(transform.forward, targetDirection);

        if(Mathf.Abs(directionAngle) > 90f && Mathf.Abs(directionAngle) < 270f)
        {
            print("Opening from the front");
            ExitPosition = backDoorPosition;
            return frontDoorPosition.position;
        }
        else
        {
            print("Opening from the back");
            ExitPosition = frontDoorPosition;
            return backDoorPosition.position;
        }
    }
}