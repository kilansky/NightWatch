using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonPattern<AudioManager>
{
    public AudioClip menuTrack;
    public AudioClip dayTrack;
    public AudioClip nightTrack;

    public AudioClip[] testTracks;
    private int currTrack = 0;

    private AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (PlayerInputs.Instance.Hotkey1)
            PreviousTrack();

        if (PlayerInputs.Instance.Hotkey2)
            NextTrack();
    }

    public void PlayDayTrack()
    {
        audioSource.clip = dayTrack;
        audioSource.Play();
        Debug.Log("Now Playing " + testTracks[currTrack].name);
    }

    public void PlayNightTrack()
    {
        audioSource.clip = nightTrack;
        audioSource.Play();
        Debug.Log("Now Playing " + testTracks[currTrack].name);
    }

    public void NextTrack()
    {
        currTrack++;
        if (currTrack > testTracks.Length - 1)
            currTrack = 0;

        audioSource.clip = testTracks[currTrack];
        audioSource.Play();
        Debug.Log("Now Playing " + testTracks[currTrack].name);
    }

    public void PreviousTrack()
    {
        currTrack--;
        if (currTrack < 0)
            currTrack = testTracks.Length - 1;

        audioSource.clip = testTracks[currTrack];
        audioSource.Play();
        Debug.Log("Now Playing " + testTracks[currTrack].name);
    }
}
