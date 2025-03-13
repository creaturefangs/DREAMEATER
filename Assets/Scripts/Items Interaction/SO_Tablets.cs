using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Tablet", order = 3)]

public class SO_Tablets : ScriptableObject
{
    public string titleText;
    
    public string[] contents;

    public Sprite icon;
}
