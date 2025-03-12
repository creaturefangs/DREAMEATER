using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dialogue", order = 4)]
public class SO_Dialogue : ScriptableObject
{
    public string characterName;
    public Sprite characterPortrait; // Icon for the scroll
    public string[] dialogueLines;
    public AudioClip dialogueSFX;
}
