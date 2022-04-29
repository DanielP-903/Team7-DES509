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
    private GameManager m_gameManagerRef;
    private TeaMaker m_teaMakerRef;

    void Start()
    {
        m_heldLocationRef = GameObject.FindGameObjectWithTag("HoldLocation");
        if (m_name == "")
        {
            m_name = m_type.ToString();
        }
        if (GameObject.FindGameObjectWithTag("GameManager"))
        {
            m_gameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        else
        {
            Debug.LogError("ERROR: Game Manager has no tag assigned!");
            Debug.DebugBreak();
        }
        if (GameObject.FindGameObjectWithTag("Machine"))
        {
            m_teaMakerRef = GameObject.FindGameObjectWithTag("Machine").GetComponent<TeaMaker>();
        }
        else
        {
            Debug.LogError("ERROR: Tea Machine has no tag assigned!");
            Debug.DebugBreak();
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

        if (m_teaMakerRef.hasClickedBrew)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = true;
        }
    }
}

