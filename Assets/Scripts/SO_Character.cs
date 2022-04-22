using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumDialogue;

[CreateAssetMenu(fileName = "Character", menuName = "Character", order = 1)]
public class SO_Character : ScriptableObject
{
    public CharacterName characterName;
    public Material material;
    public Material backMaterial;
    public string favouriteRecipe = "";
    public List<QD_Dialogue> dialogues;

}