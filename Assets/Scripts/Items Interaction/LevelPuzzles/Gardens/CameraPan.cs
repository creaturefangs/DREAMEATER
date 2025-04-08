using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public Transform playerTarget;
    public float panDuration = 1.5f;

    private Camera mainCam;
    private bool isPanning = false;

    private void Start()
    {
        mainCam = Camera.main;
    }

    public void PanToTarget(Transform target, System.Action onComplete = null)
    {
        if (!isPanning)
            StartCoroutine(PanRoutine(target.position, onComplete));
    }

    private System.Collections.IEnumerator PanRoutine(Vector3 targetPos, System.Action onComplete)
    {
        isPanning = true;

        Vector3 start = mainCam.transform.position;
        Vector3 end = new Vector3(targetPos.x, targetPos.y, start.z);

        float time = 0f;
        while (time < panDuration)
        {
            time += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(start, end, time / panDuration);
            yield return null;
        }

        mainCam.transform.position = end;
        yield return new WaitForSeconds(1f); // Pause on door

        // Return to player
        Vector3 returnTarget = new Vector3(playerTarget.position.x, playerTarget.position.y, start.z);
        time = 0f;
        while (time < panDuration)
        {
            time += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(end, returnTarget, time / panDuration);
            yield return null;
        }

        mainCam.transform.position = returnTarget;
        isPanning = false;

        onComplete?.Invoke();
    }
}

