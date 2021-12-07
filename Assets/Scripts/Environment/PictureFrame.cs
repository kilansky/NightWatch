using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureFrame : MonoBehaviour
{
    public Material[] pictures;
    public MeshRenderer meshRenderer;

    //Choose a random picture material from the pictures array and apply it
    void Start()
    {
        Material randPicture = pictures[Random.Range(0, pictures.Length)];
        meshRenderer.material = randPicture;
    }
}
