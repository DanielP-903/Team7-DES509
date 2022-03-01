using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeaMaker : MonoBehaviour
{
    private List<GameObject> m_addedIngredients = new List<GameObject>();
    [SerializeField] private int m_capacity = 4;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private TextMeshProUGUI m_recipeText;
    private Recipe m_recipeListRef;
    List<string> names = new List<string>();

    private Dictionary<Ingredient, int> m_dictionary = new Dictionary<Ingredient, int>();


    // Start is called before the first frame update
    void Start()
    {
        m_recipeListRef = GameObject.FindGameObjectWithTag("RecipeList").GetComponent<Recipe>();
        m_recipeText.text = "";
    }

    //public bool AddIngredient(GameObject ingredient)
    //{
    //    if (ingredient.GetComponent<Ingredient>() && m_addedIngredients.Count < m_capacity)
    //    {
    //        m_addedIngredients.Add(ingredient);
    //        SearchForNewRecipes();
    //        return true;
    //    }
    //    return false;
    //}


    //public void RemoveIngredient()
    //{
    //    if (m_addedIngredients.Count > 0)
    //    {
    //        m_addedIngredients.RemoveAt(m_addedIngredients.Count - 1);
    //        SearchForNewRecipes();
    //    }
    //}


    public void AddIngredient(Ingredient ingredient)
    {
        if (!m_dictionary.ContainsKey(ingredient)){
            m_dictionary.Add(ingredient, 1);
            SearchForNewRecipes();
        }
        else{
            m_dictionary[ingredient]++;
        }
    }
    public void RemoveIngredient(Ingredient ingredient)
    {
        if (!m_dictionary.ContainsKey(ingredient))
        {
            Debug.LogError("Cannot remove an ingredient that isn't in the dictionary");
        }
        else
        {
            m_dictionary.Remove(ingredient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Text.text = m_addedIngredients.Count + " / " + m_capacity;
    }

    void SearchForNewRecipes()
    {

        foreach (var recipe in m_recipeListRef.m_recipes)
        {
            bool validRecipe = true;
            foreach (var item in recipe.m_ingredients)
            {
                if (m_dictionary.ContainsKey(item.GetComponent<Ingredient>()))
                {
                    //m_dictionary[item.GetComponent<Ingredient>()];
                }
            }
        }
        

        //CountOccurences();

        //string recipeFound = "";
        //int recipeScore = 0;
        //int recipeHighScore = 0;
        //List<IngredientTracking> occurences = new List<IngredientTracking>();
        //List<string> names = new List<string>();
        //foreach (var recipe in m_recipeListRef.m_recipes)
        //{
        //    recipeScore = 0;
        //    foreach (var ingredient in recipe.m_ingredients)
        //    {
        //        string testName = ingredient.GetComponent<Ingredient>().m_type.ToString();
        //        int occurs = 0;
        //        if (names.Contains(testName))
        //        {
        //            break;
        //        }
        //        names.Add(testName);
        //        foreach (var i in recipe.m_ingredients)
        //        {
        //            if (i.GetComponent<Ingredient>().m_type.ToString() == testName)
        //            {
        //                occurs++;
        //            }
        //        }
        //        occurences.Add(new IngredientTracking(testName, occurs));

        //        // Check how many times this ingredient appears in recipe

                
        //        //for (int i = recipeScore; i < m_addedIngredients.Count; i++)
        //        //{
        //        //    if (ingredient.GetComponent<Ingredient>().m_type == m_addedIngredients[i].GetComponent<Ingredient>().m_type)
        //        //    {
        //        //        recipeScore++;
        //        //        break;
        //        //    }
        //        //}
        //    }
        //    if (recipeScore > recipeHighScore && recipeScore == recipe.m_ingredients.Count)
        //    {
        //        recipeHighScore = recipeScore;
        //        recipeFound = recipe.m_name;
        //    }
        //}

        //m_recipeText.text = recipeFound;
    }

    private void CountOccurences()
    {
        foreach (var recipe in m_recipeListRef.m_recipes)
        {
            names.Clear();
            foreach (var ingredient in recipe.m_ingredients)
            {
                string detectedName = ingredient.GetComponent<Ingredient>().m_type.ToString();
                int occurs = 0;
                if (names.Contains(detectedName))
                {
                    continue;
                }
                // Continues only if new ingredient detected

                names.Add(detectedName);
                foreach (var i in recipe.m_ingredients)
                {
                    if (i.GetComponent<Ingredient>().m_type.ToString() == detectedName)
                    {
                        occurs++;
                    }
                }
                ingredient.GetComponent<Ingredient>().occurences = occurs;
            }
        }
        Debug.Log("Finished");
    }
}
