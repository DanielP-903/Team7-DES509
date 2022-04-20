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
    [Header("Properties")]
    [Tooltip("Select what ingredient this represents")]
    public IngredientType m_type;
    [Tooltip("Name of ingredient")]
    public string m_name; 
    [Tooltip("Colour of ingredient when put in tea")]
    public Color m_colour;
    [Tooltip("Description of ingredient")]
    [TextArea(5, 10)]
    public string m_description;


    //[Tooltip("Change bitterness rating")]
    //[Range(0, 10)]
    //public int m_bitterness;
    //[Tooltip("Change sweetness rating")]
    //[Range(0, 10)]
    //public int m_sweetness;
    //[Tooltip("Change fruitiness rating")]
    //[Range(0, 10)]
    //public int m_fruitiness;
    //[Tooltip("Change earthiness rating")]
    //[Range(0, 10)]
    //public int m_earthiness;
    //[Tooltip("UNUSED! Any special properties that occur when mixed (NOTE: this may only impact the output when mixed as part of a recipe)")]
    //[SerializeField] private SpecialProperties m_specialProperties;

    [HideInInspector] public bool IsHeld = false;
    private GameObject m_heldLocationRef;

    [HideInInspector] public int occurences = 0;

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

