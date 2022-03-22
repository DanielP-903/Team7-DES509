using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tea : MonoBehaviour
{
    public string m_name;
    private Color m_colour = Color.magenta;

    public void SetColour(Color colour)
    {
        m_colour = colour;
        transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = m_colour;
        GetComponent<Animator>().Play("CupFill");
    }
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
