using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomAudioController : MonoBehaviour
{
    [Header("Audio Sources (Assign in Inspector)")]
    public AudioSource musicSource;   // Main music audio source
    public AudioSource ambienceSource; // Main ambience audio source

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float ambienceVolume = 1f;
    public float fadeDuration = 1f;

    [Header("Planet Music & Ambience Clips")]
    public List<PlanetAudio> planetAudioList = new List<PlanetAudio>();

    private Dictionary<string, PlanetAudio> planetAudioDict = new Dictionary<string, PlanetAudio>();
    private Coroutine fadeCoroutineMusic;
    private Coroutine fadeCoroutineAmbience;
    private string currentPlanet = "";

    private void Start()
    {
        // Convert List to Dictionary for fast lookups
        foreach (var planet in planetAudioList)
        {
            if (!planetAudioDict.ContainsKey(planet.planetTag))
            {
                planetAudioDict.Add(planet.planetTag, planet);
            }
        }

        // If no planet is assigned at start, fade out music and ambience
        if (string.IsNullOrEmpty(currentPlanet))
        {
            musicSource.volume = 0f;
            ambienceSource.volume = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (planetAudioDict.ContainsKey(other.tag))
        {
            if (currentPlanet != other.tag)
            {
                currentPlanet = other.tag;
                PlanetAudio newPlanetAudio = planetAudioDict[other.tag];

                // Immediately assign and start music if not playing
                if (!musicSource.isPlaying || musicSource.clip != newPlanetAudio.musicClip)
                {
                    ChangeAudio(musicSource, newPlanetAudio.musicClip, musicVolume, ref fadeCoroutineMusic);
                }

                if (!ambienceSource.isPlaying || ambienceSource.clip != newPlanetAudio.ambienceClip)
                {
                    ChangeAudio(ambienceSource, newPlanetAudio.ambienceClip, ambienceVolume, ref fadeCoroutineAmbience);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == currentPlanet)
        {
            currentPlanet = "";
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
public class PlanetAudio
{
    public string planetTag;      // The tag for the planet trigger collider
    public AudioClip musicClip;   // Music for that planet
    public AudioClip ambienceClip; // Ambience for that planet
}

