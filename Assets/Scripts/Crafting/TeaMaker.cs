using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeaMaker : MonoBehaviour
{
    private List<GameObject> m_addedIngredients = new List<GameObject>();
    [SerializeField] private int m_capacity = 4;
    [SerializeField] private TextMeshProUGUI m_Text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool AddIngredient(GameObject ingredient)
    {
        if (ingredient.GetComponent<Ingredient>() && m_addedIngredients.Count < m_capacity)
        {
            m_addedIngredients.Add(ingredient);
            return true;
        }
        return false;
    }


    public void RemoveIngredient()
    {
        if (m_addedIngredients.Count > 0)
        {
            m_addedIngredients.RemoveAt(m_addedIngredients.Count - 1);
        }
    }


    // Update is called once per frame
    void Update()
    {
        m_Text.text = m_addedIngredients.Count + " / " + m_capacity;
    }
}
