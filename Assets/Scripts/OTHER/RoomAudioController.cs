using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class RoomAudioController : MonoBehaviour
{
    [Header("Audio Sources (Assign in Inspector)")]
    public AudioSource musicSource;  // AudioSource for music
    public AudioSource ambienceSource;  // AudioSource for ambience

    [Header("Audio Clips")]
    public AudioClip musicClip;  // Single music clip
    public AudioClip ambienceClip;  // Single ambience clip

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float ambienceVolume = 1f;  // Volume slider in Inspector
    public float fadeDuration = 1f;  // Time to fade in/out

    private bool isPlaying = false;

    private void Start()
    {
        ApplyVolume();
        AssignAudioClips();
    }

    private void Update()
    {
        ApplyVolume();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlaying)
        {
            isPlaying = true;
            StartCoroutine(FadeInAudio());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlaying)
        {
            isPlaying = false;
            StartCoroutine(FadeOutAudio());
        }
    }

    private IEnumerator FadeInAudio()
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.Play();
            StartCoroutine(FadeAudioSource(musicSource, fadeDuration, ambienceVolume));
        }

        if (ambienceSource != null && ambienceClip != null)
        {
            ambienceSource.Play();
            StartCoroutine(FadeAudioSource(ambienceSource, fadeDuration, ambienceVolume));
        }
        yield return null;
    }

    private IEnumerator FadeOutAudio()
    {
        if (musicSource != null)
            StartCoroutine(FadeAudioSource(musicSource, fadeDuration, 0f, stopAfterFade: true));

        if (ambienceSource != null)
            StartCoroutine(FadeAudioSource(ambienceSource, fadeDuration, 0f, stopAfterFade: true));

        yield return null;
    }

    private IEnumerator FadeAudioSource(AudioSource source, float duration, float targetVolume, bool stopAfterFade = false)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
        if (stopAfterFade)
            source.Stop();
    }

    private void ApplyVolume()
    {
        if (musicSource != null) musicSource.volume = ambienceVolume;
        if (ambienceSource != null) ambienceSource.volume = ambienceVolume;
    }

    private void AssignAudioClips()
    {
        if (musicSource != null && musicClip != null)
            musicSource.clip = musicClip;

        if (ambienceSource != null && ambienceClip != null)
            ambienceSource.clip = ambienceClip;
    }
}
