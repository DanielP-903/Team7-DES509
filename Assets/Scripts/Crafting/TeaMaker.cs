using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System;
using System.Runtime.Serialization;

[Serializable]
public class CustomIntDictionary : SerializableDictionary<UnityEngine.Object, int> { }

public class TeaMaker : MonoBehaviour
{
    private List<GameObject> m_addedIngredients = new List<GameObject>();
    [SerializeField] private int m_capacity = 4;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private TextMeshProUGUI m_recipeText;
    private Recipe m_recipeListRef;
    private int m_total = 0;
    List<string> names = new List<string>();
    [SerializeField]
    CustomIntDictionary m_container;
    public IDictionary<UnityEngine.Object, int> CustomIntDictionary
    {
        get { return m_container; }
        set { m_container.CopyFrom(value); }
    }

    [SerializeField]
    private CustomIntDictionary m_FoundRecipes;


    // Start is called before the first frame update
    void Start()
    {
        m_recipeListRef = GameObject.FindGameObjectWithTag("RecipeList").GetComponent<Recipe>();
        m_recipeText.text = "";
    }

    public void AddIngredient(UnityEngine.Object ingredient)
    {
        if (m_total + 1 <= m_capacity)
        {
            if (!m_container.ContainsKey(ingredient))
            {
                m_container.Add(ingredient, 1);
            }
            else
            {
                m_container[ingredient]++;
            }
            SearchForNewRecipes();
        }
    }
    public void RemoveIngredient(UnityEngine.Object ingredient)
    {
        if (!m_container.ContainsKey(ingredient))
        {
            Debug.LogError("Cannot remove an ingredient that isn't in the dictionary");
        }
        else
        {
            m_container.Remove(ingredient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_total = 0;
        foreach (var item in m_container)
        {
            m_total += item.Value;
        }
        m_Text.text = m_total + " / " + m_capacity;
    }

    void SearchForNewRecipes()
    {
        m_recipeText.text = "";
        foreach (var recipe in m_recipeListRef.m_recipes)
        {
            int occurs = 0;
            bool validRecipe = true;
            foreach (var item in m_container)
            {
                if (recipe.m_ingredients.ContainsKey(item.Key))
                {
                    if (recipe.m_ingredients[item.Key] != item.Value)
                    {
                        Debug.Log("We don't got it bois");
                        validRecipe = false;

                    }
                    else
                    {
                        occurs++;
                    }
                }
            }
            if (occurs == recipe.m_ingredients.Count)
            {
                m_recipeText.text = "FOUND: " + recipe.m_name;
            }
            Debug.Log("occurs: " + occurs);
            Debug.Log("recipe.m_ingredients.Count: " + recipe.m_ingredients.Count);
        }
    }
}