using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public float openAnimationDuration;
    public float closeAnimationDuration;
    public float chaseOpenDuration;
    public float chaseCloseDuration;
    public float DoorOpenDuration; //Temporary
    public GameObject DoorAnimator;
    public Collider DoorCollider;
    public GameObject uiNotification;
    public GameObject RoomSize;
    

    [HideInInspector]  public bool IsOpened;
    [HideInInspector]  public bool IsClosed;
    [HideInInspector] public Transform ExitPosition;
    [HideInInspector] public float lowerXBoundary;
    [HideInInspector] public float upperXBoundary;
    [HideInInspector] public float lowerZBoundary;
    [HideInInspector] public float upperZBoundary;

    [SerializeField] private Transform frontDoorPosition;
    [SerializeField] private Transform backDoorPosition;
    private Animator myAnimator;

    private void Awake()
    {
        IsOpened = false;
        IsClosed = true;
        myAnimator = DoorAnimator.GetComponent<Animator>();

        lowerXBoundary = RoomSize.transform.position.x - (RoomSize.transform.localScale.x / 2);
        upperXBoundary = RoomSize.transform.position.x + (RoomSize.transform.localScale.x / 2);
        lowerZBoundary = RoomSize.transform.position.z - (RoomSize.transform.localScale.z / 2);
        upperZBoundary = RoomSize.transform.position.z + (RoomSize.transform.localScale.z / 2);
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
        print("Open Door being called");
        if (IsClosed)
        {
            print("Door Opens");
            myAnimator.SetFloat("BaseSpeed", (1 / openAnimationDuration));
            myAnimator.SetTrigger("OpenDoor");
            IsOpened = true;
            IsClosed = false;
            uiNotification.SetActive(false);
            DoorCollider.enabled = !DoorCollider.enabled;
            StartCoroutine(DoorClosing());
            //print("Door Opens");
        }
        
    }

    public void CloseDoor()
    {
        if(IsOpened)
        {
            myAnimator.SetFloat("BaseSpeed", (1 / closeAnimationDuration));
            myAnimator.SetTrigger("CloseDoor");
            
            //print("Door Closes");
        }
    }

    public void ChaseOpenDoor()
    {
        print("Open Door being called");
        if (IsClosed)
        {
            print("Door Opens");
            myAnimator.SetFloat("BaseSpeed", (1 / chaseOpenDuration));
            myAnimator.SetTrigger("OpenDoor");
            IsOpened = true;
            IsClosed = false;
            uiNotification.SetActive(false);
            DoorCollider.enabled = !DoorCollider.enabled;
            StartCoroutine(DoorClosing());
            // 
        }

    }

    /*public void ChaseCloseDoor()
    {
        if (IsOpened)
        {
            myAnimator.SetFloat("BaseSpeed", (1 / chaseCloseDuration));
            myAnimator.SetTrigger("CloseDoor");
            IsOpened = false;
            IsClosed = true;
            print("Door Closes");
        }
    }*/

    private IEnumerator DoorClosing()
    {
        //print("Closing Door");
        yield return new WaitForSeconds(DoorOpenDuration);
        DoorCollider.enabled = !DoorCollider.enabled;
        CloseDoor();

        yield return new WaitForSeconds(1 / closeAnimationDuration);
        IsOpened = false;
        IsClosed = true;
        
    }

    public Vector3 GetWaitPosition(Vector3 position)
    {
        Vector3 targetDirection = transform.position - position;

        float directionAngle = Vector3.Angle(transform.forward, targetDirection);

        if(Mathf.Abs(directionAngle) > 90f && Mathf.Abs(directionAngle) < 270f)
        {
            //print("Go to front");
            ExitPosition = backDoorPosition;
            return frontDoorPosition.position;
        }
        else
        {
            //print("Go to back");
            ExitPosition = frontDoorPosition;
            return backDoorPosition.position;
        }
    }

    public void CheckDoorPosition(GameObject agent)
    {
        print("Check Door Position");
        Vector3 targetDirection = transform.position - agent.transform.position;

        float directionAngle = Vector3.Angle(transform.forward, targetDirection);

        if (Mathf.Abs(directionAngle) > 90f && Mathf.Abs(directionAngle) < 270f)
        {
            if (gameObject.GetComponent<GuardPathfinding>())
            {
                print("Facing Front");
                agent.GetComponent<GuardPathfinding>().facingFrontDoor = true;
            }
        }
        else
        {
            if (gameObject.GetComponent<GuardPathfinding>())
            {
                agent.GetComponent<GuardPathfinding>().facingFrontDoor = false;
            }
            
        }
    }

    
}
