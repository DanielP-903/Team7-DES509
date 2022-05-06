using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldIngredient : MonoBehaviour 
{
    public GameObject linkedIngredient;

    [HideInInspector] public bool IsHeld = false;
    private GameObject m_heldLocationRef;
    private GameManager m_gameManagerRef;
    private TeaMaker m_teaMakerRef;
    // Start is called before the first frame update
    void Start()
    {
        m_heldLocationRef = GameObject.FindGameObjectWithTag("HoldLocation");
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

    // Update is called once per frame
    void Update()
    {
        if (IsHeld)
        {
            //if (linkedIngredient.GetComponent<Ingredient>().m_type == IngredientType.Cindershard)
            //{
            //    transform.position = m_heldLocationRef.transform.position;
            //}
            //else
            //{
            //    transform.position = m_heldLocationRef.transform.position - new Vector3(0,0,0.2f);
            //}
            transform.position = m_heldLocationRef.transform.position;

            // transform.rotation = m_heldLocationRef.transform.rotation;
            transform.parent = m_heldLocationRef.transform;
        }
        else
        {
            transform.parent = null;
        }

       // GetComponent<BoxCollider>().enabled = !m_teaMakerRef.hasClickedBrew;
    }
}
