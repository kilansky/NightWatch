using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LayerMask laserHitMask;
    public LayerMask thiefMask;

    public GameObject laserAlert;
    public Vector3 alertOffset = new Vector3(0, 0.5f, 0);
    public float alarmSoundInterval = 1f;

    private LineRenderer lineRenderer;
    private GameObject spawnedAlert;
    private AudioSource audioSource;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //Draw laser line renderer
        if (Physics.Raycast(ray, out hit, 50f, laserHitMask))
        {
            lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));

            //Check for thieves
            if (Physics.Raycast(ray, out hit, 50f, thiefMask))
            {
                LaserTriggered();
            }
        }
    }

    //Activate when a thief walks in front of the laser
    private void LaserTriggered()
    {
        if (spawnedAlert)
            return;

        spawnedAlert = Instantiate(laserAlert, transform.position + alertOffset, Quaternion.identity);
        StartCoroutine(SoundAlarm());
    }

    private IEnumerator SoundAlarm()
    {
        for (int i = 0; i < 3; i++)
        {
            audioSource.Play();
            yield return new WaitForSeconds(alarmSoundInterval);
        }
        DeactivateAlert();
    }

    public void DeactivateAlert()
    {
        Destroy(spawnedAlert, 3f);
    }
}
