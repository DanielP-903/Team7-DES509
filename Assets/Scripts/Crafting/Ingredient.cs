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

    /// <summary>
    /// NO LONGER REQUIRED - MODEL AS PART OF INGREDIANT OBJECT
    /// </summary>
    //[Tooltip("Assign model of ingredient")]
    //[SerializeField] private GameObject m_model;

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
    [Tooltip("Any special properties that occur when mixed (NOTE: this may only impact the output when mixed as part of a recipe)")]
    [SerializeField] private SpecialProperties m_specialProperties;

    public bool IsHeld = false;
    private GameObject m_heldLocationRef;

    void Start()
    {
        m_heldLocationRef = GameObject.FindGameObjectWithTag("HoldLocation");
    }

    void Update()
    {
        if (IsHeld)
        {
            transform.position = m_heldLocationRef.transform.position;
            transform.rotation = m_heldLocationRef.transform.rotation;
            transform.parent = m_heldLocationRef.transform;
            
        }
        else
        {
            transform.parent = null;
        }
    }
}
