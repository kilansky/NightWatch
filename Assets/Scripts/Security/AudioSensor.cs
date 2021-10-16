using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSensor : MonoBehaviour
{
    public float detectionRange = 5f;

    public GameObject audioAlert;
    public Vector3 alertOffset = new Vector3(0, 0.5f, 0);
    public float alarmSoundInterval = 1f;

    private GameObject spawnedAlert;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //Set scale of object to size of the detection range
        transform.localScale = new Vector3(detectionRange, transform.localScale.y, detectionRange);
    }

    //Check for any thieves that enter the sphere collider of this audio sensor
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ThiefPathfinding>())
        {
            AudioSensorTriggered();
        }
    }

    //Activate when a thief walks in front of the laser
    private void AudioSensorTriggered()
    {
        if (spawnedAlert)
            return;

        spawnedAlert = Instantiate(audioAlert, transform.position + alertOffset, Quaternion.identity);
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