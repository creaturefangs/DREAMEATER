using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Scroll", order = 4)]

public class SO_Items : MonoBehaviour
{

    // Dialogue Text and description

    public string itemText;
    public Sprite itemIcon; 
    public string[] itemLines;
    public AudioClip iteminteractSFX;

    //SFX and ingame-data

    public AudioClip itemSFX;

    // inventory information

    public string itemDescription;


}
