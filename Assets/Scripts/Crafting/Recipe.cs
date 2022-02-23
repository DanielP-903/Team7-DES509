using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredients
{
    [Tooltip("Name given to the recipe")]
    [SerializeField] public string m_name;
    [Tooltip("Ingredients that make up the recipe")]
    [SerializeField] public List<GameObject> m_ingredients = new List<GameObject>();
}

public class Recipe : MonoBehaviour
{
    [SerializeField] public List<RecipeIngredients> m_recipes = new List<RecipeIngredients>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
