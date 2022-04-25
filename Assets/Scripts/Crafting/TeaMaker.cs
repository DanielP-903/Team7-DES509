using System;
using System.Collections;
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



    [Tooltip("Model prefab for the tea cup")]
    public GameObject m_teaModel;

    private GameObject m_recipeBase;
    private GameObject m_lid;
    private GameObject m_lidDestination;

    // New Recipe Found
    private GameObject m_foundObject;
    private TextMeshProUGUI m_foundText;


    private int m_discoveredRecipesNo = 0;
    [HideInInspector] public Recipe m_currentlyCalculatedRecipe;
    
    private RecipeList m_recipeListRef;
    public int Total = 0;
    private readonly Stack<UnityEngine.Object> AddedOrder = new Stack<UnityEngine.Object>();

    public Transform m_cupStartPoint;
    public Transform m_cupEndPoint;
    private GameManager m_gameManagerRef;
    [SerializeField]
    public CustomIntDictionary m_container;

    public bool hasClickedBrew = false;

    private PlayerController m_playerRef;

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
        m_gameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        m_playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_lid = transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        m_lidDestination = transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        m_Text = mainCanvas.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        m_recipeText = mainCanvas.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();

        m_recipeBase = mainCanvas.transform.GetChild(6).transform.GetChild(2).gameObject;

        m_recipeListRef = GameObject.FindGameObjectWithTag("RecipeList").GetComponent<RecipeList>();
        m_recipeText.text = "";

        m_foundObject = mainCanvas.transform.GetChild(5).gameObject;
        m_foundText = m_foundObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        foreach (var item in m_recipeListRef.m_recipes)
        {
            if (!m_discoveredRecipes.ContainsKey(item.m_name))
            {
                m_discoveredRecipes.Add(item.m_name, false);
            }
        }

        m_Text.gameObject.SetActive(false);
        m_recipeText.gameObject.SetActive(false);

        m_foundObject.SetActive(false);
        m_teaModel = Instantiate(m_teaModel, transform.parent);
        m_teaModel.SetActive(true);
    }
    void OnDrawGizmos()
    {
        var color = Color.cyan;
        color.a = 0.5f;
        Gizmos.color = color;

        Gizmos.DrawMesh(m_teaModel.GetComponent<MeshFilter>().sharedMesh, 0, m_teaModel.transform.position, m_teaModel.transform.rotation, m_teaModel.transform.localScale );

        color = Color.red;
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawSphere(m_cupEndPoint.position, 0.1f);
    }
    public GameObject FindIngredient(GameObject toFind)
    {
        foreach (var ingredient in m_gameManagerRef.m_ingredientList)
        {
            if (ingredient.GetComponent<Ingredient>().m_type == toFind.GetComponent<Ingredient>().m_type)
            {
                return ingredient;
            }
        }

        return null;
    }

    private IEnumerator displayNewRecipeFoundPopup(float waitTime) 
    {
        m_foundObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        m_foundObject.SetActive(false);
    }

    public int GetContainerCount()
    {
        return m_container.Count;
    }
    public void BrewTea()
    {
        if (m_container.Count > 0 && AddedOrder.Count > 0)
        {
            m_playerRef.sfx_pouring.Play();
            hasClickedBrew = true;
            m_teaModel.SetActive(true);
            m_teaModel.transform.GetChild(0).gameObject.SetActive(true);
            if (m_currentlyCalculatedRecipe.m_name != null)
            {
                m_teaModel.GetComponent<Tea>().m_name = m_currentlyCalculatedRecipe.m_name;
                if (m_discoveredRecipes.ContainsKey(m_currentlyCalculatedRecipe.m_name))
                {
                    if (m_discoveredRecipes[m_currentlyCalculatedRecipe.m_name] == false)
                    {
                        // UPDATE RECIPE BOOK UI
                        m_discoveredRecipes[m_currentlyCalculatedRecipe.m_name] = true;
                        m_discoveredRecipesNo++;
                        GameObject listTheRecipe = Instantiate(m_recipeBase, m_recipeBase.transform.parent);

                        listTheRecipe.GetComponent<RectTransform>().position = new Vector3(
                            m_recipeBase.GetComponent<RectTransform>().position.x + (m_discoveredRecipesNo > 3 ? (m_discoveredRecipesNo-4) * 260.0f : (m_discoveredRecipesNo-1) * 260.0f),
                            m_recipeBase.GetComponent<RectTransform>().position.y +((m_discoveredRecipesNo > 3 ? 1 : 0) * -150.0f), 
                            m_recipeBase.GetComponent<RectTransform>().position.z);

                        //m_recipeBase.GetComponent<RectTransform>().position.x +((m_discoveredRecipesNo - 1 * (-maxHorizontalSpread * (int)((m_discoveredRecipesNo - 1) / maxHorizontalSpread))) * 220.0f),
                        //m_recipeBase.GetComponent<RectTransform>().position.y +((int)((m_discoveredRecipesNo-1)/maxHorizontalSpread) * -150.0f), 
                        //listTheRecipe.GetComponent<RectTransform>().offsetMax = new Vector2(
                        //    m_recipeBase.GetComponent<RectTransform>().offsetMax.x + Mathf.Abs(((m_discoveredRecipesNo-1 * (-maxHorizontalSpread *(int)((m_discoveredRecipesNo-1)/maxHorizontalSpread)))) * 515.0f),
                        //    m_recipeBase.GetComponent<RectTransform>().offsetMin.y + Mathf.Abs((int)((m_discoveredRecipesNo-1)/maxHorizontalSpread) * -420.0f));
                        listTheRecipe.SetActive(true);
                        listTheRecipe.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Name: " + m_currentlyCalculatedRecipe.m_name + "\nDescription: " + m_currentlyCalculatedRecipe.m_description;
                        foreach (var item in m_currentlyCalculatedRecipe.m_ingredients)
                        {
                            listTheRecipe.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += "\n" + item.Key.name + " x " + item.Value;
                        }

                        Debug.Log("Recipe Discovered: " + m_currentlyCalculatedRecipe.m_name);
                        m_foundText.text = "New Recipe Discovered: " + m_currentlyCalculatedRecipe.m_name;
                        StartCoroutine(displayNewRecipeFoundPopup(5.0f));
                        m_teaModel.GetComponent<Tea>().SetColour(m_currentlyCalculatedRecipe.m_colour);

                        m_playerRef.sfx_newRecipe.Play();
                    }
                    else
                    {
                        m_playerRef.sfx_normal.Play();
                    }

                    m_currentlyCalculatedRecipe = null;
                    m_lid.SetActive(false);
                    m_lidDestination.SetActive(true);
                    m_container.Clear();
                    AddedOrder.Clear();

                    
                }
                else
                {
                    Debug.LogError("OOPS! THIS RECIPE DOESN'T EXIST IN m_discoveredRecipes!");
                }
            }
            else
            {
                // Not a defined tea but that's a-ok!
                m_teaModel.GetComponent<Tea>().m_name = "regular";
                Color combinedColour = new Color(0, 0, 0, 1);
                foreach (var item in m_container)
                {
                    combinedColour += FindIngredient((GameObject)item.Key).GetComponent<Ingredient>().m_colour;
                }
                m_teaModel.GetComponent<Tea>().SetColour(combinedColour);            
                m_lid.SetActive(false);
                m_lidDestination.SetActive(true);
                m_container.Clear();
                AddedOrder.Clear();

                m_playerRef.sfx_normal.Play();
                
            }
        }
    }

    public void BrewTeaFromUI(string RecipeName)
    {
        // In case of need for implementation?
    }

    public void GiveTea()
    {
        if (m_teaModel.activeInHierarchy)
        {
            // Slide tea to character
            // Do dialogue response
            m_lid.SetActive(true);
            m_lidDestination.SetActive(false);
        }
        m_teaModel.GetComponent<Tea>().IsHeld = true;
        hasClickedBrew = false;
    }

    public void ResetTea()
    {
        m_currentlyCalculatedRecipe = null;
        m_teaModel.SetActive(true);
        m_teaModel.transform.position = m_cupStartPoint.transform.position;
        m_teaModel.transform.rotation = m_cupStartPoint.transform.rotation;
    }

    public void AddIngredient(UnityEngine.Object ingredient)
    {
        if (Total + 1 <= m_capacity)
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
            AddedOrder.Clear();
            m_container.Clear();

            Debug.LogWarning("Stack is empty!");        
        }
    }

    // Update is called once per frame
    void Update()
    {
        Total = 0;
        foreach (var item in m_container)
        {
            Total += item.Value;
        }
        m_Text.text = Total + " / " + m_capacity;
    }

    void SearchForNewRecipes()
    {
        m_recipeText.text = "";
        m_currentlyCalculatedRecipe = new Recipe();
        foreach (var recipe in m_recipeListRef.m_recipes)
        {
            int occurs = 0;
            foreach (var item in m_container)
            {
                if (recipe.m_ingredients.ContainsKey(item.Key))
                {
                    if (recipe.m_ingredients[item.Key] == item.Value)
                    {
                        occurs+=item.Value;
                        if (occurs == CalculateTotal(recipe))
                            break;
                    }
                }
            }
            if (occurs == CalculateTotal(recipe))
            {
                m_recipeText.text = "FOUND: " + recipe.m_name;
                m_currentlyCalculatedRecipe.m_name = recipe.m_name;
                m_currentlyCalculatedRecipe.m_ingredients = recipe.m_ingredients;
                m_currentlyCalculatedRecipe.m_colour = recipe.m_colour;
                m_currentlyCalculatedRecipe.m_description = recipe.m_description;
            }
        }
    }

    private float CalculateTotal(Recipe r)
    {
        float total = 0;
        foreach (var item in r.m_ingredients)
        {
            // NEXT TODO
            total += item.Value;
        }
        return total;
    }
}