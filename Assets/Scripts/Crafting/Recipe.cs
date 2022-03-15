using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredients
{
    [Tooltip("Name given to the recipe")]
    [SerializeField] public string m_name;
    //[Tooltip("Ingredients that make up the recipe")]
    //[SerializeField] public List<GameObject> m_ingredients = new List<GameObject>();

    [SerializeField]
    public CustomIntDictionary m_ingredients;
    public IDictionary<UnityEngine.Object, int> CustomIntDictionary
    {
        get { return m_ingredients; }
        set { m_ingredients.CopyFrom(value); }
    }
}

public class Recipe : MonoBehaviour
{
    [SerializeField] public List<RecipeIngredients> m_recipes = new List<RecipeIngredients>();
}