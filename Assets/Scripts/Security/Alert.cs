using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Alert : MonoBehaviour
{
    public GameObject audioAlert;
    public Vector3 alertOffset = new Vector3(0, 0.5f, 0);
    public float alarmSoundInterval = 1f;

    private GameObject spawnedAlert;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //Activate when a thief walks in the detection area of the sensor
    public void SensorTriggered()
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
