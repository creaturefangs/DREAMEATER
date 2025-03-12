using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dialogue", order = 4)]
public class SO_Dialogue : ScriptableObject
{
    public string characterName;
    public Sprite characterPortrait; // Icon for the scroll
    public string[] dialogueLines;
    public AudioClip dialogueSFX;
    public TMP_FontAsset font;  // Reference to the custom font
    public int fontSize = 36;   // Default font size
}
