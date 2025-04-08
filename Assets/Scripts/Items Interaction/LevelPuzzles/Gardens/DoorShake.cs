using UnityEngine;

public class DoorShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeAmount = 0.2f;

    private Vector3 originalPos;

    public void ShakeDoor()
    {
        originalPos = transform.localPosition;
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    private System.Collections.IEnumerator Shake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeAmount;
            float offsetY = Random.Range(-1f, 1f) * shakeAmount;
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

