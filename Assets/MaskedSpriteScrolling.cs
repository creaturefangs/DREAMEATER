using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MaskedSpriteScrolling : MonoBehaviour
{
        [Header("Scroll Settings")]
        public Vector2 scrollSpeed = new Vector2(0.5f, 0f); // units per second (u,v)
        public Vector2 tileScale = new Vector2(1f, 1f);     // how many times the texture repeats across the sprite

        [Header("Material Options")]
        [Tooltip("If true, the script will instantiate a unique material for this renderer (safe). " +
                 "If false, it will modify the shared material (affects all objects using that material).")]
        public bool useInstanceMaterial = true;

        SpriteRenderer sr;
        Material mat; // instance or shared
        Vector2 offset;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            // Ensure a material is assigned (Sprites/Default recommended)
            if (sr.sharedMaterial == null)
            {
                Debug.LogWarning("SpriteRenderer has no material. Assign a material using Sprites/Default.");
            }

            if (useInstanceMaterial)
            {
                // create an instance so changes only affect this object
                mat = Instantiate(sr.sharedMaterial);
                sr.material = mat;
            }
            else
            {
                // modify sharedMaterial (affects all sprites using the same material)
                mat = sr.sharedMaterial;
            }

            // Set initial tiling (scale) if possible
            if (mat != null)
            {
                // _MainTex is the texture property used by Sprites/Default
                mat.SetTextureScale("_MainTex", tileScale);
                // ensure any existing offset is loaded
                offset = mat.GetTextureOffset("_MainTex");
            }
        }

        void Update()
        {
            if (mat == null) return;

            // update offset
            offset += scrollSpeed * Time.deltaTime;

            // keep offset inside 0..1 to avoid runaway values
            offset.x = offset.x % 1f;
            offset.y = offset.y % 1f;

            mat.SetTextureOffset("_MainTex", offset);
        }

        void OnValidate()
        {
            // make changes visible in editor when tweaking fields
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var m = useInstanceMaterial ? sr.material : sr.sharedMaterial;
                if (m != null)
                {
                    m.SetTextureScale("_MainTex", tileScale);
                    m.SetTextureOffset("_MainTex", Vector2.zero);
                }
            }
        }
}
