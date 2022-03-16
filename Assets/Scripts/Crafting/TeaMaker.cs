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

    // Debug stuff
    private TextMeshProUGUI m_Text;
    private TextMeshProUGUI m_recipeText;

    private GameObject m_recipeBase;
    public GameObject m_teaModel;
    private int m_discoveredRecipesNo = 0;
    private RecipeIngredients m_currentlyCalculatedRecipe;
    
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
        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        m_Text = mainCanvas.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        m_recipeText = mainCanvas.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

        GameObject recipeCanvas = GameObject.FindGameObjectWithTag("RecipeCanvas");
        m_recipeBase = recipeCanvas.transform.GetChild(1).gameObject;

        m_recipeListRef = GameObject.FindGameObjectWithTag("RecipeList").GetComponent<Recipe>();
        m_recipeText.text = "";

        foreach (var item in m_recipeListRef.m_recipes)
        {
            if (!m_discoveredRecipes.ContainsKey(item.m_name))
            {
                m_discoveredRecipes.Add(item.m_name, false);
            }
        }

        m_teaModel = Instantiate(m_teaModel);
        m_teaModel.SetActive(false);
    }
    void OnDrawGizmos()
    {
        var color = Color.cyan;
        color.a = 0.5f;
        Gizmos.color = color;

        Gizmos.DrawMesh(m_teaModel.GetComponent<MeshFilter>().sharedMesh, 0, m_teaModel.transform.position, m_teaModel.transform.rotation, m_teaModel.transform.localScale );// transform.rotation * Quaternion.Euler(90, 0, 0));
    }

    public void BrewTea()
    {
        m_teaModel.SetActive(true);
        if (m_currentlyCalculatedRecipe.m_name != null)
        {

                m_teaModel.GetComponent<Tea>().m_name = m_currentlyCalculatedRecipe + " tea";
                if (m_discoveredRecipes.ContainsKey(m_currentlyCalculatedRecipe.m_name))
                {
                    if (m_discoveredRecipes[m_currentlyCalculatedRecipe.m_name] == false)
                    {
                        m_discoveredRecipes[m_currentlyCalculatedRecipe.m_name] = true;
                        m_discoveredRecipesNo++;
                        GameObject listTheRecipe = Instantiate(m_recipeBase, m_recipeBase.transform.parent);
                        listTheRecipe.GetComponent<RectTransform>().offsetMax = new Vector2(-m_discoveredRecipesNo * 50, listTheRecipe.GetComponent<RectTransform>().rect.position.y);
                        listTheRecipe.SetActive(true);
                        listTheRecipe.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_currentlyCalculatedRecipe.m_name;
                        foreach (var item in m_currentlyCalculatedRecipe.m_ingredients)
                        {
                            listTheRecipe.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += "\n" + item.Key.name + " x " + item.Value;
                        }
                        Debug.Log("Recipe Discovered: " + m_currentlyCalculatedRecipe.m_name);
                    }
                }
                else
                {
                    Debug.LogError("OOPS! THIS RECIPE DOESN'T EXIST IN m_discoveredRecipes!");
                }
            
        }
       

           
        

        m_teaModel.GetComponent<Tea>().SetColour(Color.blue); // Default to magenta for now...
        //m_teaModel.GetComponent<Tea>().m_colour = new Color(0,0,0,.5f);
        m_container.Clear();
        AddedOrder.Clear();
        SearchForNewRecipes();
    }

    public void GiveTea()
    {
        if (m_teaModel.activeInHierarchy)
        {
            // Slide tea to character
            // Do dialogue response
            // 
            m_teaModel.SetActive(false);
            m_teaModel.GetComponent<Tea>().m_name = String.Empty;

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
        m_currentlyCalculatedRecipe = new RecipeIngredients();
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
                m_currentlyCalculatedRecipe.m_name = recipe.m_name;
                m_currentlyCalculatedRecipe.m_ingredients = recipe.m_ingredients;
            }
        }
    }
}