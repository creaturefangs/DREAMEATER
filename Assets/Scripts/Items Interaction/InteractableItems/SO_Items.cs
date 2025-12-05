using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Item", order = 4)]

public class SO_Items : ScriptableObject
{
    public string itemID;

    // Dialogue Text and description

    public string itemText;
    public Sprite itemIcon;
    public string[] itemLines;
    public AudioClip iteminteractSFX;
    public AudioClip typeSFX;

    // Hidden item support
    public bool isHiddenItem = false;             // NEW
    public string hiddenItemMessage = "";         // NEW

    //SFX and ingame-data

    public AudioClip itemSFX;

    // inventory information

    public string itemDescription;


}
