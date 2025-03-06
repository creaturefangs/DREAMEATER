using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelationEffect : MonoBehaviour
{
    public Material pixelationMaterial;
    [Range(1, 200)] public int pixelSize = 50;
    [Range(0, 1)] public float ditherStrength = 0.2f;
    public bool pixelSnap = true; // Enable pixel snapping

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelationMaterial != null)
        {
            pixelationMaterial.SetFloat("_PixelSize", pixelSize);
            pixelationMaterial.SetFloat("_DitherStrength", ditherStrength);

            if (pixelSnap)
            {
                Vector3 pos = transform.position;
                pos.x = Mathf.Round(pos.x * pixelSize) / pixelSize;
                pos.y = Mathf.Round(pos.y * pixelSize) / pixelSize;
                transform.position = pos;
            }

            Graphics.Blit(source, destination, pixelationMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
