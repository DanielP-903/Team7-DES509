using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum IngredientType
{
    Basic, Peppermint, Lemon, Rooibos  // Needs to be updated
}

enum SpecialProperties
{
    None, Energy, Calming, Soothing, Healing  // Needs to be updated
}

public class Ingredient : MonoBehaviour
{
    [Header("Basic Setup")]
    [Tooltip("Select what ingredient this represents")]
    [SerializeField] private IngredientType m_type;
    [Tooltip("Assign model of ingredient")]
    [SerializeField] private GameObject m_model;

    [Header("Properties")]
    [Tooltip("Change bitterness rating")]
    [Range(0, 10)]
    [SerializeField] private int m_bitterness;
    [Tooltip("Change sweetness rating")]
    [Range(0, 10)]
    [SerializeField] private int m_sweetness;
    [Tooltip("Change fruitiness rating")]
    [Range(0, 10)]
    [SerializeField] private int m_fruitiness;
    [Tooltip("Change earthiness rating")]
    [Range(0, 10)]
    [SerializeField] private int m_earthiness;
    [Tooltip("Any special properties that occur when mixed")]
    [SerializeField] private SpecialProperties m_specialProperties;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
