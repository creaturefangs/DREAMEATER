using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOutlineHover : MonoBehaviour
{
        private SpriteRenderer spriteRenderer;
        private Material originalMaterial;
        public Material outlineMaterial; // Assign an outline material in the Inspector

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalMaterial = spriteRenderer.material;
            }
        }

        public void OnMouseEnter()
        {
            if (spriteRenderer != null && outlineMaterial != null)
            {
                spriteRenderer.material = outlineMaterial;
            }
        }

        public void OnMouseExit()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.material = originalMaterial;
            }
        }
}
