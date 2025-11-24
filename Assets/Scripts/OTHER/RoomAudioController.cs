using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomAudioController : MonoBehaviour
{
    [Header("Audio Sources (Assign in Inspector)")]
    public AudioSource musicSource;
    public AudioSource ambienceSource;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float ambienceVolume = 1f;
    public float fadeDuration = 1f;

    [Header("Room Audio List (Matched By Trigger Name)")]
    public List<RoomAudioEntry> roomAudioList = new List<RoomAudioEntry>();

    private Dictionary<string, RoomAudioEntry> roomDict = new Dictionary<string, RoomAudioEntry>();

    private Coroutine fadeCoroutineMusic;
    private Coroutine fadeCoroutineAmbience;

    private string currentRoom = "";

    private void Start()
    {
        // Build lookup dictionary using room names as keys
        foreach (var entry in roomAudioList)
        {
            if (!roomDict.ContainsKey(entry.roomName))
                roomDict.Add(entry.roomName, entry);
        }

        // Initially silent
        musicSource.volume = 0f;
        ambienceSource.volume = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string roomName = other.gameObject.name;

        if (roomDict.ContainsKey(roomName))
        {
            if (currentRoom == roomName)
                return;

            currentRoom = roomName;
            RoomAudioEntry entry = roomDict[roomName];

            // Fade in music
            if (musicSource.clip != entry.musicClip)
                ChangeAudio(musicSource, entry.musicClip, musicVolume, ref fadeCoroutineMusic);

            // Fade in ambience
            if (ambienceSource.clip != entry.ambienceClip)
                ChangeAudio(ambienceSource, entry.ambienceClip, ambienceVolume, ref fadeCoroutineAmbience);
        }
        else
        {
            Debug.LogWarning($"RoomAudioController: No audio entry for Trigger name: {roomName}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == currentRoom)
        {
            currentRoom = "";

            StartCoroutine(FadeOutAudio(musicSource, fadeDuration));
            StartCoroutine(FadeOutAudio(ambienceSource, fadeDuration));
        }
    }

    private void ChangeAudio(AudioSource source, AudioClip newClip, float targetVolume, ref Coroutine fadeCoroutine)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInAudio(source, newClip, targetVolume));
    }

    private IEnumerator FadeInAudio(AudioSource source, AudioClip newClip, float targetVolume)
    {
        // Fade out current clip first
        if (source.isPlaying)
            yield return StartCoroutine(FadeOutAudio(source, fadeDuration));

        source.clip = newClip;
        source.Play();

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, time / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    private IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }
}

[System.Serializable]
public class RoomAudioEntry
{
    public string roomName;      // Must match the trigger GameObject name
    public AudioClip musicClip;
    public AudioClip ambienceClip;
}

