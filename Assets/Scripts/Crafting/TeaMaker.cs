using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CustomIntDictionary : SerializableDictionary<UnityEngine.Object, int> { }
[Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool> { }

public class TeaMaker : MonoBehaviour
{

    [SerializeField] private int m_capacity = 4;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private TextMeshProUGUI m_recipeText;

    private string m_currentlyCalculatedRecipe;

    private Recipe m_recipeListRef;
    private int m_total = 0;
    private readonly Stack<UnityEngine.Object> AddedOrder = new Stack<UnityEngine.Object>();
    [SerializeField]
    CustomIntDictionary m_container;
    public IDictionary<UnityEngine.Object, int> CustomIntDictionary
    {
        get { return m_container; }
        set { m_container.CopyFrom(value); }
    }

    [SerializeField]
    StringBoolDictionary m_discoveredRecipes;
    public IDictionary<string, bool> StringBoolDictionary
    {
        get { return m_discoveredRecipes; }
        set { m_discoveredRecipes.CopyFrom(value); }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_recipeListRef = GameObject.FindGameObjectWithTag("RecipeList").GetComponent<Recipe>();
        m_recipeText.text = "";

        foreach (var item in m_recipeListRef.m_recipes)
        {
            if (!m_discoveredRecipes.ContainsKey(item.m_name))
            {
                m_discoveredRecipes.Add(item.m_name, false);
            }
        }
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
            AddedOrder.Push(ingredient);
            SearchForNewRecipes();
        }
    }
    public void RemoveIngredient()
    {
        if (AddedOrder.Count > 0)
        {
            UnityEngine.Object toBeRemoved = AddedOrder.Pop();
            if (!m_container.ContainsKey(toBeRemoved))
            {
                Debug.LogError("Cannot remove an ingredient that isn't in the dictionary, re-adding to stack");
                AddedOrder.Push(toBeRemoved);
            }
            else
            {
                if (m_container[toBeRemoved] > 0)
                {
                    m_container[toBeRemoved]--;
                }
                else
                {
                    m_container.Remove(toBeRemoved);
                }
            }
            SearchForNewRecipes();
        }
        else
        {
            Debug.LogWarning("Stack is empty!");        
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
        m_currentlyCalculatedRecipe = null;
        foreach (var recipe in m_recipeListRef.m_recipes)
        {
            int occurs = 0;
            foreach (var item in m_container)
            {
                if (recipe.m_ingredients.ContainsKey(item.Key))
                {
                    if (recipe.m_ingredients[item.Key] == item.Value)
                    {
                        occurs++;
                    }
                }
            }
            if (occurs == recipe.m_ingredients.Count)
            {
                m_recipeText.text = "FOUND: " + recipe.m_name;
                m_currentlyCalculatedRecipe = recipe.m_name;
                if (m_discoveredRecipes.ContainsKey(recipe.m_name))
                {
                    if (m_discoveredRecipes[recipe.m_name] == false)
                    {
                        m_discoveredRecipes[recipe.m_name] = true;
                        Debug.Log("Recipe Discovered: " + recipe.m_name);
                    }
                }
                else
                {
                    Debug.LogError("OOPS! THIS RECIPE DOESN'T EXIST IN m_discoveredRecipes!");
                }
            }
            //Debug.Log("occurs: " + occurs);
            //Debug.Log("recipe.m_ingredients.Count: " + recipe.m_ingredients.Count);
        }
    }
}