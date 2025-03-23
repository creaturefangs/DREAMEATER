using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class DoorSceneChanger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad; // Scene to load

    [Header("Fade Settings")]
    public Image fadeImage; // Assign a full-screen UI Image
    public float fadeDuration = 1f; // Duration of fade effect

    private void Start()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0; // Ensure the fade starts fully transparent
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(false); // Disable initially
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !string.IsNullOrEmpty(sceneToLoad))
        {
            StartCoroutine(LoadSceneWithFade());
        }
    }

    private IEnumerator LoadSceneWithFade()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true); // Enable fade image
            yield return StartCoroutine(Fade(0, 1)); // Fade in
        }

        SceneManager.LoadScene(sceneToLoad); // Load new scene
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}
