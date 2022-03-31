using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 1)]
public class SO_Character : ScriptableObject
{
    public CharacterName characterName;
    public Material material;
    public int stage = 0;
    public string favouriteRecipe = "";
}