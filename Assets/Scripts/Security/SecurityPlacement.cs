using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityPlacement : SingletonPattern<SecurityPlacement>
{
    public GameObject cctvCamera;
    public GameObject laserSensor;
    public GameObject guard;
    public GameObject audioSensor;

    public LayerMask floorMask;
    public LayerMask wallMask;
    public LayerMask placeableMask;
    public LayerMask placementBlockedMask;
    public Material greenHighlight;
    public Material redHighlight;

    [HideInInspector] public bool placementMode = false;
    [HideInInspector] public bool movementMode = false;
    [HideInInspector] public bool placementSkillGate = false;
    [HideInInspector] public bool cancelPlacementSkillGate = false;
    [HideInInspector] public bool insufficientFundsTooltip = false;
    [HideInInspector] public GameObject heldObject;
    [HideInInspector] public int heldObjectCost;

    private List<Material> originalMats = new List<Material>();
    private enum materialState {Red, Green, Original};
    private materialState currMaterial = materialState.Original;
    private bool originalMatsStored = false;
    private Vector3 movedObjectOriginalPos;
    private Quaternion movedObjectOriginalRot;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if an object has been selected to place, and the mouse is not over UI       
        if ((placementMode || movementMode) && !MouseInputUIBlocker.Instance.blockedByUI)
            SetAimTargetPosition();
    }

    //Perform a raycast to set the position of the selected object
    private void SetAimTargetPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        //Determine whether the held object should be placed on walls or the floor
        bool placeOnWalls = heldObject.GetComponent<SecurityMeasure>().placedOnWalls;

        //Check if the player right clicks to exit placement mode
        if (PlayerInputs.Instance.RightClickPressed)
        {
            if (placementMode)
            {
                ExitPlacementMode();
                return;
            }
            else if (movementMode)
            {
                CancelMoving();
                return;
            }
        }

        //Check if the player has insufficient funds. If they do, show a tooltip
        CheckToShowFundsTooltip();

        //Check to store the mats of the held object
        if (!originalMatsStored)
            StoreOriginalMaterials();

        //check if mouse is over a placable area, and the object to place is placed on walls
        if ((Physics.Raycast(ray, out hit, Mathf.Infinity, placeableMask)) && placeOnWalls)
        {
            Ray rayZ = new Ray(hit.point - (hit.transform.forward * 0.3f), hit.transform.forward);//draw ray forward (z)
            //Ray rayX = new Ray(hit.point - (hit.transform.right * 0.3f), hit.transform.right);//draw ray right (x)
            RaycastHit hitZ;
            //RaycastHit hitX;

            float rayZDist = Mathf.Infinity;
            //float rayXDist = Mathf.Infinity;

            //Get the nearest point of the wall to place objects on
            Debug.DrawRay(hit.point - (hit.transform.forward * 0.3f), hit.transform.forward, Color.red);
            if (Physics.Raycast(rayZ, out hitZ, 3f, wallMask))
                rayZDist = hitZ.distance;

            /* //Code for better placement accuracy in corners - needs adjustment
            Debug.DrawRay(hit.point - (hit.transform.right * 0.3f), hit.transform.right, Color.red);
            if (Physics.Raycast(rayX, out hitX, 3f, wallMask))
                rayXDist = hitX.distance;

            if (rayXDist < rayZDist)
                hitZ = hitX;
            */

            //Set the position and rotation of the held object to snap onto the wall
            heldObject.transform.position = hitZ.point;
            SetPlacementRotation(hitZ.normal);

            //check if mouse is NOT over a non-placable area and the Cancel Placement Skill Gate is not active
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, placementBlockedMask) && 
                !Physics.Raycast(rayZ, out hitZ, 3f, placementBlockedMask) && !cancelPlacementSkillGate)
            {
                //If not overlapping with another object, and is affordable, allow placement on wall
                if (!heldObject.transform.GetChild(0).GetComponent<OverlapDetection>().isOverlapping
                    && (movementMode || MoneyManager.Instance.money >= heldObjectCost))
                {
                    SetPlacementMaterial("green");

                    //Click to place on wall
                    if (PlayerInputs.Instance.LeftClickPressed)
                        PlaceSecurityMeasure();

                    return;
                }
            }
        }
        //check if mouse is over the floor
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            heldObject.transform.position = hit.point;

            //check if held object is not placed on walls (ie: Guards), is not overlapping with other things, and is affordable
            if (!placeOnWalls && !heldObject.transform.GetChild(0).GetComponent<OverlapDetection>().isOverlapping
                && (movementMode || MoneyManager.Instance.money >= heldObjectCost))
            {
                //Check if held object is over a nav mesh area
                NavMeshHit NavIsHit;
                int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask))
                {
                    SetPlacementMaterial("green");

                    //Click to place on floor
                    if (PlayerInputs.Instance.LeftClickPressed)
                        PlaceSecurityMeasure();

                    return;
                }
            }
        }

        SetPlacementMaterial("red");
    }

    //Rotate an object to snap properly to a wall
    private void SetPlacementRotation(Vector3 hitNormal)
    {
        Quaternion cameraRotation = Quaternion.Euler(0f, 0f, 0f);

        if (Mathf.RoundToInt(hitNormal.x) == -1) //Face East
            cameraRotation = Quaternion.Euler(0f, 90f, 0f);
        else if (Mathf.RoundToInt(hitNormal.x) == 1) //Face West
            cameraRotation = Quaternion.Euler(0f, -90f, 0f);
        else if (Mathf.RoundToInt(hitNormal.z) == -1) //Face South
            cameraRotation = Quaternion.Euler(0f, 0f, 0f);
        else if (Mathf.RoundToInt(hitNormal.z) == 1) //Face North
            cameraRotation = Quaternion.Euler(0f, 180f, 0f);
        else
        {
            cameraRotation = Quaternion.Euler(0f, 90f, 0f);
            Debug.Log("Invaild wall normal to place the camera on");
        }

        heldObject.transform.rotation = cameraRotation;
    }

    //Store the materials of all children of the game object, so that they may be reset later
    public void StoreOriginalMaterials()
    {
        originalMats.Clear();

        int i = 0;
        foreach (Transform child in heldObject.transform)
        {
            //Verify that that current child has a mesh renderer
            if (child.GetComponent<MeshRenderer>())
            {
                //Store all materials attached to the child's mesh renderer
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                foreach (Material mat in meshRenderer.materials)
                {
                    originalMats.Add(mat);
                    i++;
                }
            }
            //Check if child has children of its own
            if (child.childCount > 0)
            {
                foreach (Transform child2 in child)
                {
                    //Check if second child has mesh renderer
                    if (child2.GetComponent<MeshRenderer>())
                    {
                        //Store all materials attached to the second child's mesh renderer
                        MeshRenderer meshRenderer = child2.GetComponent<MeshRenderer>();
                        foreach (Material mat in meshRenderer.materials)
                        {
                            originalMats.Add(mat);
                            i++;
                        }
                    }
                }
            }
        }
        originalMatsStored = true;
        currMaterial = materialState.Original;
        //Debug.Log("Materials Stored");
    }

    //Set all materials of the object back to the stored original materials
    private void SetOriginalMaterial()
    {
        int i = 0;
        foreach (Transform child in heldObject.transform)
        {
            //Verify that that current child has a mesh renderer
            if (child.GetComponent<MeshRenderer>())
            {
                //Set all materials attached to the child's mesh renderer
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                List<Material> materialsToSet = new List<Material>();
                foreach (Material mat in meshRenderer.materials)
                {
                    materialsToSet.Add(originalMats[i]);
                    i++;
                }
                meshRenderer.materials = materialsToSet.ToArray();
            }
        
            //Check if child has children of its own
            if (child.childCount > 0)
            {
                foreach (Transform child2 in child)
                {
                    //Check if second child has mesh renderer
                    if (child2.GetComponent<MeshRenderer>())
                    {
                        //Set all materials attached to the second child's mesh renderer
                        MeshRenderer meshRenderer = child2.GetComponent<MeshRenderer>();
                        List<Material> materialsToSet = new List<Material>();
                        foreach (Material mat in meshRenderer.materials)
                        {
                            materialsToSet.Add(originalMats[i]);
                            i++;
                        }
                        meshRenderer.materials = materialsToSet.ToArray();
                    }
                }
            }
        }
        originalMats.Clear();
        originalMatsStored = false;
    }

    //Change the material of the held object to green, red, or the original material
    private void SetPlacementMaterial(string materialToSet)
    {
        //If the material to set is already applied, exit this function
        if (materialToSet == "green" && currMaterial == materialState.Green)
            return;
        else if (materialToSet == "red" && currMaterial == materialState.Red)
            return;
        else if (materialToSet == "original" && currMaterial == materialState.Original)
            return;

        //Set this child's material to green
        if (materialToSet == "green")
        {
            SetMaterial(greenHighlight);
            currMaterial = materialState.Green;
        }
        //Set this child's material to red
        else if (materialToSet == "red")
        {
            SetMaterial(redHighlight);
            currMaterial = materialState.Red;
        }
        //Set this child's material to its original mat
        else if (materialToSet == "original")
        {
            SetOriginalMaterial();
            currMaterial = materialState.Original;
        }
    }

    //Set all materials of the object to the given material
    private void SetMaterial(Material matToSet)
    {
        foreach (Transform child in heldObject.transform)
        {
            //Verify that that current child has a mesh renderer
            if (child.GetComponent<MeshRenderer>())
            {
                //Set all materials attached to the child's mesh renderer
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                List <Material> materialsToSet = new List<Material>();
                foreach (Material mat in meshRenderer.materials)
                    materialsToSet.Add(matToSet);

                meshRenderer.materials = materialsToSet.ToArray();
            }

            //Check if child has children of its own
            if (child.childCount > 0)
            {
                foreach (Transform child2 in child)
                {
                    //Check if second child has mesh renderer
                    if (child2.GetComponent<MeshRenderer>())
                    {
                        //Set all materials attached to the second child's mesh renderer
                        //Set all materials attached to the child's mesh renderer
                        MeshRenderer meshRenderer = child2.GetComponent<MeshRenderer>();
                        List<Material> materialsToSet = new List<Material>();
                        foreach (Material mat in meshRenderer.materials)
                            materialsToSet.Add(matToSet);

                        meshRenderer.materials = materialsToSet.ToArray();
                    }
                }
            }
        }
    }

    //Turn off placement mode and remove the held object
    public void ExitPlacementMode()
    {
        placementMode = false;
        SecuritySelection.Instance.canSelect = true;

        //Remove held object
        if (heldObject != null)
            Destroy(heldObject);

        //Deactivate the insufficent funds tooltip
        if(insufficientFundsTooltip)
        {
            TooltipSystem.Instance.HideTooltip();
            insufficientFundsTooltip = false;
        }

        if (cancelPlacementSkillGate) //If in the tutorial, right click to exit placement mode and activate the selection skill gate
        {
            //TutorialController.Instance.NextButton();
            //TutorialController.Instance.SelectionSkillGate();
            cancelPlacementSkillGate = false;
            DialogueManager.Instance.StartNextDialogue();
        }
    }

    //Called when a security measure is selected and the 'move' button is pressed
    public void MovePlacedObject()
    {
        //Get selected object and close the selection
        GameObject objectToMove = SecuritySelection.Instance.selectedObject.gameObject;
        SecuritySelection.Instance.CloseSelection();
        SecuritySelection.Instance.canSelect = false;

        if (objectToMove.GetComponent<NavMeshAgent>())
            objectToMove.GetComponent<NavMeshAgent>().enabled = false;

        //If the object to move is a guard, disable the model mesh
        if (objectToMove.GetComponent<GuardPathfinding>())
            objectToMove.transform.GetChild(1).GetComponent<CapsuleCollider>().isTrigger = true;

        //Begin moving the object
        movedObjectOriginalPos = objectToMove.transform.position;
        movedObjectOriginalRot = objectToMove.transform.rotation;
        heldObject = objectToMove;
        movementMode = true;
        HUDController.Instance.EnableButtons(false, false, false, false); //disable security buttons
        HUDController.Instance.nightWatchButton.interactable = false;

        //Store original material of the object to move
        StoreOriginalMaterials();
        currMaterial = materialState.Original;
    }

    //Cancels the movement of a security measure and places it back where it started
    private void CancelMoving()
    {
        //Reset material and positions
        SetPlacementMaterial("original");
        currMaterial = materialState.Original;
        heldObject.transform.position = movedObjectOriginalPos;
        heldObject.transform.rotation = movedObjectOriginalRot;
        heldObject = null;
        movementMode = false;

        SecuritySelection.Instance.canSelect = true; //enable selections
        HUDController.Instance.EnableButtons(true, true, true, true); //enable security buttons
        HUDController.Instance.nightWatchButton.interactable = true;

        //If the object to move is a guard, enable the model mesh
        if (heldObject.GetComponent<GuardPathfinding>())
            heldObject.transform.GetChild(1).GetComponent<CapsuleCollider>().isTrigger = false;
    }

    //Place a security measure into the level
    private void PlaceSecurityMeasure()
    {
        SetPlacementMaterial("original");

        if(placementMode)//Place a new security measure
        {
            GameObject newObject = Instantiate(heldObject, heldObject.transform.position, heldObject.transform.rotation);
            MoneyManager.Instance.SubtractMoney(heldObjectCost);

            //If the placed object was a guard, do some setup (ex: set patrol route color)
            if (newObject.GetComponent<GuardPatrolPoints>())
            {
                newObject.transform.GetChild(1).GetComponent<CapsuleCollider>().isTrigger = false; //enable collider
                newObject.GetComponent<GuardPatrolPoints>().SetGuardPatrolColor(); //set guard color
                Color guardColor = newObject.GetComponent<GuardPatrolPoints>().patrolMarkerColor; //get guard color
                GuardController.Instance.ActivateGuardPanel(guardColor, newObject.GetComponent<GuardPathfinding>()); //activate HUD UI on night canvas
                HUDController.Instance.SetNightWatchButtonInteractability(); //Enable the Begin Night Watch Button
            }

            //If placing a hackable security measure in the first level, adjust it to be unhackable
            if (GameManager.Instance.currentLevel == 1 && newObject.GetComponent<HackedSecurityScript>())
                newObject.GetComponent<HackedSecurityScript>().hackResistance = 3;

            if (placementSkillGate) //If in the tutorial, placing an object will move to the next panel and activate the exit placement skill gate
            {
                //TutorialController.Instance.NextButton();
                placementSkillGate = false;

                //If a camera is being placed on the first level, disable the camera buy button
                if(newObject.GetComponent<SecurityMeasure>().securityType == SecurityMeasure.SecurityType.camera && GameManager.Instance.currentLevel == 1)
                    HUDController.Instance.SetPlanningUIActive(false, false, false);

                SkillCheckManager.Instance.cctvPlacementArrow.SetActive(false);
                DialogueManager.Instance.StartNextDialogue();
            }
        }
        else if(movementMode)//Place a moved security measure
        {
            movementMode = false;
            SecuritySelection.Instance.canSelect = true; //enable selections
            HUDController.Instance.EnableButtons(true, true, true, true); //enable security buttons
            HUDController.Instance.nightWatchButton.interactable = true;

            if (heldObject.GetComponent<NavMeshAgent>())
                heldObject.GetComponent<NavMeshAgent>().enabled = true;

            heldObject = null;
        }
    }

    //Checks if the player has insufficient funds to place the item that is held. If they do, show a tooltip, otherwise hide it
    private void CheckToShowFundsTooltip()
    {
        //If the player cannot afford another of this security measure, show the insufficent funds tooltip
        if (!insufficientFundsTooltip && placementMode && MoneyManager.Instance.money < heldObjectCost)
        {
            TooltipSystem.Instance.ShowTooltip("", "Insufficient Funds");
            insufficientFundsTooltip = true;
        }
        //If the player can afford another of this security measure, hide the insufficent funds tooltip
        else if (insufficientFundsTooltip && MoneyManager.Instance.money >= heldObjectCost)
        {
            TooltipSystem.Instance.HideTooltip();
            insufficientFundsTooltip = false;
        }
    }
}