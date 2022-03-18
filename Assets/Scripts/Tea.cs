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
}
