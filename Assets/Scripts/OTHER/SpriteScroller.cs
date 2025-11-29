using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    [SerializeField] private float speedX = 0.5f;
    [SerializeField] private float speedY = 0f;
    private Renderer rend;
    private Vector2 offset;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;
        // Uses the material instance on this renderer (for testing only)
        rend.material.mainTextureOffset = offset;
    }
}
