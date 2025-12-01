using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Scroll", order = 4)]

public class SO_Scrolls : ScriptableObject
{

    public string titleText;
    public Sprite icon; // Icon for the scroll
    public string[] dialogueLines;
    public AudioClip dialogueSFX;

}
