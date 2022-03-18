using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    [Tooltip("Name given to the recipe")]
    [SerializeField] public string m_name;
    [Tooltip("Colour of this recipe's brewed tea")]
    [SerializeField] public Color m_colour;

    [SerializeField]
    public CustomIntDictionary m_ingredients;
    public IDictionary<UnityEngine.Object, int> CustomIntDictionary
    {
        get { return m_ingredients; }
        set { m_ingredients.CopyFrom(value); }
    }
}

public class RecipeList : MonoBehaviour
{
    [SerializeField] public List<Recipe> m_recipes = new List<Recipe>();
}