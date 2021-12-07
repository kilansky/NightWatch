using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonPattern<AudioManager>
{
    public AudioClip dayTrack;
    public AudioClip nightTrack;

    private AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDayTrack()
    {
        //audioSource.Stop();
        audioSource.clip = dayTrack;
        audioSource.Play();
    }

    public void PlayNightTrack()
    {
        audioSource.Stop();
        audioSource.clip = nightTrack;
        audioSource.Play();
    }
}
