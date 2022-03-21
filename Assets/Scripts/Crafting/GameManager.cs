using System.Collections.Generic;
using UnityEngine;

public enum CharacterName
{
    Shylo, Mimi, Docorty
}
[System.Serializable]
public struct ingredientListValues
{
    public GameObject ingredient;
    public Color colour;
}

public class GameManager : MonoBehaviour
{
   public List<GameObject> m_ingredientList;

   // Game Flags
   public static bool m_hasBrewedATea = false;

   public CharacterName currentCharacter = CharacterName.Shylo;



}
