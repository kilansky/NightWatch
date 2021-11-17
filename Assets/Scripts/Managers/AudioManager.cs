using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonPattern<AudioManager>
{
    public AudioClip menuTrack;
    public AudioClip dayTrack;
    public AudioClip nightTrack;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDayTrack()
    {
        audioSource.PlayOneShot(dayTrack);
    }

    public void PlayNightTrack()
    {
        audioSource.PlayOneShot(nightTrack);
    }
}
