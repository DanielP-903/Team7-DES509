using UnityEngine;

public enum IngredientType
{
    Peppermint, FruitSlice, Rosehip, Ginger, Blossoms, Licorice  // Needs to be updated
}

enum SpecialProperties
{
    None, Energy, Calming, Soothing, Healing  // Needs to be updated
}

public class Ingredient : MonoBehaviour
{
    [Header("Basic Setup")]
    [Tooltip("Select what ingredient this represents")]
    [SerializeField] public IngredientType m_type;
    [Tooltip("Name of ingredient")]
    [SerializeField] private string m_name;
    /// <summary>
    /// NO LONGER REQUIRED - MODEL AS PART OF INGREDIANT OBJECT
    /// </summary>
    //[Tooltip("Assign model of ingredient")]
    //[SerializeField] private GameObject m_model;

    [Header("Properties")]
    [Tooltip("Colour of ingredient when put in tea")]
    public Color m_colour;
    [Tooltip("Change bitterness rating")]
    [Range(0, 10)]
    public int m_bitterness;
    [Tooltip("Change sweetness rating")]
    [Range(0, 10)]
    public int m_sweetness;
    [Tooltip("Change fruitiness rating")]
    [Range(0, 10)]
    public int m_fruitiness;
    [Tooltip("Change earthiness rating")]
    [Range(0, 10)]
    public int m_earthiness;
    [Tooltip("Any special properties that occur when mixed (NOTE: this may only impact the output when mixed as part of a recipe)")]
    [SerializeField] private SpecialProperties m_specialProperties;

    public bool IsHeld = false;
    private GameObject m_heldLocationRef;

    public int occurences = 0;


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

