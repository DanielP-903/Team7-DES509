using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tea : MonoBehaviour
{
    public string m_name;
    private Color m_colour = Color.magenta;

    void Update()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = m_colour;
    }

    public void SetColour(Color colour)
    {
        m_colour = colour;
    }
}
