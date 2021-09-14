using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (placementMode)
            SetAimTargetPosition();
    }

    private void SetAimTargetPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.AimPosition);
        RaycastHit hit;

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
            SetPlacementMaterial("green");

            if (PlayerInputs.Instance.LeftClickReleased)
            {
                SetPlacementMaterial("original");
                Instantiate(heldObject, heldObject.transform.position, heldObject.transform.rotation);
            }
        }
        //check if mouse is over the floor
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            heldObject.transform.position = hit.point;

            //check if held object is not placed on walls (ie: Guards)
            if(!placeOnWalls)
            {
                SetPlacementMaterial("green");

                if (PlayerInputs.Instance.LeftClickReleased)
                {
                    SetPlacementMaterial("original");
                    Instantiate(heldObject, heldObject.transform.position, heldObject.transform.rotation);
                }
            }
            else
                SetPlacementMaterial("red");
        }
    }

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

    private void StoreOriginalMaterials()
    {
        int i = 0;
        foreach (Transform child in heldObject.transform)
        {
            //Verify that that current child has a mesh renderer
            if (child.GetComponent<MeshRenderer>())
            {
                originalMats.Add(child.GetComponent<MeshRenderer>().material);
                Debug.Log("originalMats is: " + originalMats[i]);
                i++;
            }
        }
        originalMatsStored = true;
    }

    //Change the material of the held object to green, red, or the original material
    private void SetPlacementMaterial(string materialToSet)
    {
        int i = 0;
        //Get all children of the held object
        foreach (Transform child in heldObject.transform)
        {
            //Verify that that current child has a mesh renderer
            if (child.GetComponent<MeshRenderer>())
            {
                //Set this child's material to green
                if(materialToSet == "green" && currMaterial != materialState.Green)
                    child.GetComponent<MeshRenderer>().material = greenHighlight;

                //Set this child's material to red
                else if (materialToSet == "red" && currMaterial != materialState.Red)
                    child.GetComponent<MeshRenderer>().material = redHighlight;

                //Set this child's material to its original mat
                else if (materialToSet == "original" && currMaterial != materialState.Original)
                {
                    Debug.Log("originalMats is: " + originalMats);
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
            originalMats.Clear();
            originalMatsStored = false;
        }
    }
}
