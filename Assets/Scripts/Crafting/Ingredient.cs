using UnityEngine;

public enum IngredientType
{
    Breezeleaf, GlowLime, HeartOfRose, Cindershard, PurpleCrystal, Ebonstraw  // Needs to be updated
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

    [HideInInspector] public bool IsHeld = false;
    private GameObject m_heldLocationRef;

    void Start()
    {
        m_heldLocationRef = GameObject.FindGameObjectWithTag("HoldLocation");
        if (m_name == "")
        {
            m_name = m_type.ToString();
        }
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

