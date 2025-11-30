using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeedX = 0.2f;
    [SerializeField] private float scrollSpeedY = 0.0f;

    private Material _mat;
    private Vector2 _offset;

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _mat = sr.material;   // makes a unique instance of the material
    }

    void Update()
    {
        _offset.x += scrollSpeedX * Time.deltaTime;
        _offset.y += scrollSpeedY * Time.deltaTime;

        _mat.mainTextureOffset = _offset;
    }
}
