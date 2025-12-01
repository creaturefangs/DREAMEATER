using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [HideInInspector] public AudioSource musicSource;
    [HideInInspector] public AudioSource ambienceSource;

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


    private void Awake()
    {
        // Try to find music source in the scene
        GameObject musicObj = GameObject.FindGameObjectWithTag("MusicAudio");
        if (musicObj != null)
            musicSource = musicObj.GetComponent<AudioSource>();

        // Try to find ambience source in the scene
        GameObject ambienceObj = GameObject.FindGameObjectWithTag("AmbienceAudio");
        if (ambienceObj != null)
            ambienceSource = ambienceObj.GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Build dictionary
        foreach (var entry in roomAudioList)
        {
            if (!roomDict.ContainsKey(entry.roomName))
                roomDict.Add(entry.roomName, entry);
        }

        // Initially silent
        musicSource.volume = 0f;
        ambienceSource.volume = 0f;

        // NEW: Check if the player is already inside a trigger
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        foreach (var hit in hits)
        {
            string roomName = hit.gameObject.name;

            if (roomDict.ContainsKey(roomName))
            {
                currentRoom = roomName;
                RoomAudioEntry entry = roomDict[roomName];

                // Start both audio tracks immediately (fade in)
                ChangeAudio(musicSource, entry.musicClip, musicVolume, ref fadeCoroutineMusic);
                ChangeAudio(ambienceSource, entry.ambienceClip, ambienceVolume, ref fadeCoroutineAmbience);

                break;
            }
        }
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

