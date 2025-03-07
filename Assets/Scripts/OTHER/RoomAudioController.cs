using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomAudioController : MonoBehaviour
{
    [Header("Audio Clips for Layered Ambience")]
    public List<AudioClip> ambienceClips = new List<AudioClip>();

    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Start()
    {
        foreach (AudioClip clip in ambienceClips)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = false;
            audioSources.Add(source);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (AudioSource source in audioSources)
            {
                source.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (AudioSource source in audioSources)
            {
                source.Stop();
            }
        }
    }
}
