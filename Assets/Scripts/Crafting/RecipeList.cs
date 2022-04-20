using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    [Tooltip("Name given to the recipe")]
    public string m_name;
    [Tooltip("Colour of this recipe's brewed tea")]
    public Color m_colour;
    [Tooltip("Description of this recipe")]
    [TextArea(5, 10)]
    public string m_description;

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