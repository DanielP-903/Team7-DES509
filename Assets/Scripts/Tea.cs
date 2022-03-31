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
    private TeaMaker m_teaMakerRef;
    private PlayerController m_playerRef;
    void Start()
    {
        m_heldLocationRef = GameObject.FindGameObjectWithTag("HoldLocation");
        m_teaMakerRef = GameObject.FindGameObjectWithTag("Machine").GetComponent<TeaMaker>();
        m_playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (IsHeld)
        {
            transform.position = m_heldLocationRef.transform.position;
            transform.rotation = m_heldLocationRef.transform.rotation;
            transform.position = new Vector3(m_heldLocationRef.transform.position.x, m_heldLocationRef.transform.position.y - 0.4f, m_heldLocationRef.transform.position.z);
            transform.rotation = Quaternion.Euler(-90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            transform.parent = m_heldLocationRef.transform;
            //if (m_playerRef.transform.position == m_playerRef.m_lockTalkPos)
            if (Vector3.Distance(m_playerRef.transform.position, m_playerRef.m_lockTalkPos) < 0.1f)
            {
                IsHeld = false;
                transform.position = m_teaMakerRef.m_cupEndPoint.position;

            }
        }
        else
        {
            transform.parent = null;
           // if (!GameManager.m_hasBrewedATea && m_playerRef.m_teaAnimFlag) 
            if (!GameManager.m_hasBrewedATea)
            {
                transform.position = m_teaMakerRef.m_cupStartPoint.position;
            }
            else
            {
                //transform.position = m_teaMakerRef.m_cupEndPoint.position;
            }

            if (m_playerRef.transform.position == m_playerRef.m_lockTalkPos)
            {
                transform.position = m_teaMakerRef.m_cupEndPoint.position;
            }
        }
    }
}
