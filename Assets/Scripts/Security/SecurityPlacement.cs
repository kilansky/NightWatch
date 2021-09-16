using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SecurityPlacement : SingletonPattern<SecurityPlacement>
{
    public GameObject cctvPrefab;
    public GameObject laserPrefab;
    public GameObject guardPrefab;
    public GameObject audioPrefab;
    public LayerMask floorMask;
    public LayerMask wallMask;
    public LayerMask placeableMask;
    public Material greenHighlight;
    public Material redHighlight;

    [HideInInspector] public bool placementMode = false;
    [HideInInspector] public bool placeOnWalls = true;
    [HideInInspector] public GameObject heldObject;

    private List<Material> originalMats = new List<Material>();
    private enum materialState {Red, Green, Original};
    private materialState currMaterial = materialState.Original;
    private bool originalMatsStored = false;
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
        if (placementMode && !EventSystem.current.IsPointerOverGameObject())
            SetAimTargetPosition();
    }

    //Perform a raycast to set the position of the selected object
    private void SetAimTargetPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        //Check if the player right clicks to exit placement mode
        if (PlayerInputs.Instance.RightClickPressed)
            ExitPlacementMode();

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

            /*
            Debug.DrawRay(hit.point - (hit.transform.right * 0.3f), hit.transform.right, Color.red);
            if (Physics.Raycast(rayX, out hitX, 3f, wallMask))
                rayXDist = hitX.distance;

            if (rayXDist < rayZDist)
                hitZ = hitX;
            */

            //Set the position and rotation of the held object to snap onto the wall

            heldObject.transform.position = hitZ.point;
            SetPlacementRotation(hitZ.normal);

            //If overlapping with another object, prevent placement on wall
            if (heldObject.transform.GetChild(0).GetComponent<OverlapDetection>().isOverlapping)
            {
                SetPlacementMaterial("red");
            }
            else //If not overlapping, allow placement on wall
            {
                SetPlacementMaterial("green");

                //Click to place on wall
                if (PlayerInputs.Instance.LeftClickPressed)
                {
                    SetPlacementMaterial("original");
                    Instantiate(heldObject, heldObject.transform.position, heldObject.transform.rotation);
                }
            }
        }
        //check if mouse is over the floor
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            heldObject.transform.position = hit.point;

            //check if held object is not placed on walls (ie: Guards) and is not overlapping with other things
            if(!placeOnWalls && !heldObject.transform.GetChild(0).GetComponent<OverlapDetection>().isOverlapping)
            {
                SetPlacementMaterial("green");

                //Click to place on floor
                if (PlayerInputs.Instance.LeftClickPressed)
                {
                    SetPlacementMaterial("original");
                    Instantiate(heldObject, heldObject.transform.position, heldObject.transform.rotation);
                }
            }
            else //Prevent object from being placed on floor
                SetPlacementMaterial("red");
        }
    }

    //Turn off placement mode and remove the held object
    public void ExitPlacementMode()
    {
        SecurityPlacement.Instance.placementMode = false;

        //Remove held object
        if (heldObject != null)
            Destroy(heldObject);
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
            Debug.LogError("Invaild wall normal to place the camera on");

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
                originalMats.Add(child.GetComponent<MeshRenderer>().material);
                i++;
            }
        }
        originalMatsStored = true;
        //Debug.Log("Materials Stored");
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

        int i = 0;
        //Get all children of the held object
        foreach (Transform child in heldObject.transform)
        {
            //Verify that that current child has a mesh renderer
            if (child.GetComponent<MeshRenderer>())
            {
                //Set this child's material to green
                if(materialToSet == "green")
                    child.GetComponent<MeshRenderer>().material = greenHighlight;

                //Set this child's material to red
                else if (materialToSet == "red")
                    child.GetComponent<MeshRenderer>().material = redHighlight;

                //Set this child's material to its original mat
                else if (materialToSet == "original")
                {
                    child.GetComponent<MeshRenderer>().material = originalMats[i];
                    i++;
                }
            }
        }

        if(materialToSet == "green")
            currMaterial = materialState.Green;
        else if (materialToSet == "red")
            currMaterial = materialState.Red;
        else if (materialToSet == "original")
            currMaterial = materialState.Original;

        //If the material is now the original material, clear the originalMats list
        if (currMaterial == materialState.Original)
        {
            //Debug.Log("Object set to Original mats, Stored Mats Cleared");
            originalMats.Clear();
            originalMatsStored = false;
        }
    }
}