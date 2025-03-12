using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomAudioController : MonoBehaviour
{
    [Header("Audio Clips for Layered Ambience")]
    public List<AudioClip> ambienceClips = new List<AudioClip>();

    [Header("Audio Sources")]
    public AudioSource musicSource; // Public AudioSource assigned in Inspector

    private List<AudioSource> ambienceSources = new List<AudioSource>();

    private void Start()
    {
        if (musicSource == null)
        {
            Debug.LogError("Music Source is not assigned in RoomAudioController!");
            return;
        }

        // Create AudioSources for ambience using the public musicSource's settings
        foreach (AudioClip clip in ambienceClips)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = false;
            source.volume = musicSource.volume; // Match volume settings
            source.outputAudioMixerGroup = musicSource.outputAudioMixerGroup; // Match audio mixer
            ambienceSources.Add(source);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayAudio();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAudio();
        }
    }

    private void PlayAudio()
    {
        if (musicSource && !musicSource.isPlaying)
        {
            musicSource.Play();
        }

        foreach (AudioSource source in ambienceSources)
        {
            if (!source.isPlaying)
                source.Play();
        }
    }

    private void StopAudio()
    {
        if (musicSource)
        {
            musicSource.Stop();
        }

        foreach (AudioSource source in ambienceSources)
        {
            source.Stop();
        }
    }
}