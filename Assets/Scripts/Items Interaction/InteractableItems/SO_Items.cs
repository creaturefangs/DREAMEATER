using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Item", order = 4)]

public class SO_Items : ScriptableObject
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
