using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public SO_Character characterScriptableObject;

    private GameObject m_playerRef;
    private GameManager m_gameManagerRef;
    [HideInInspector] public bool isAvailable;
    [HideInInspector] public bool leaving;

    private void Start()
    {
        m_gameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        m_playerRef = GameObject.FindGameObjectWithTag("Player");
        isAvailable = false;
        leaving = false;
        transform.position = m_gameManagerRef.m_entryPos;
    }

    public void ChangeCharacter(SO_Character newCharacter)
    {
        characterScriptableObject = newCharacter;
        GetComponent<MeshRenderer>().sharedMaterial = characterScriptableObject.material;
        isAvailable = false;
        transform.position = m_gameManagerRef.m_entryPos;
    }

    private void Update()
    {
        GetComponent<BoxCollider>().enabled = isAvailable;
        transform.LookAt(m_playerRef.transform, Vector3.up);
        transform.eulerAngles = new Vector3(90.0f, transform.eulerAngles.y, 0.0f);
        if (!isAvailable)
        {
            MoveIntoPosition();
        }
    }

    private void MoveIntoPosition()
    {
        Vector3 destination = leaving ? m_gameManagerRef.m_entryPos : m_gameManagerRef.m_exitPos;
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime*2);
        if (Vector3.Distance(transform.position, destination) < 0.02f)
        {
            transform.position = destination;
            if (leaving)
            {
                leaving = false;
                isAvailable = true; // Change to true/false to allow/disallow stopping after char exit
            }
            else
            {
                isAvailable = true;
            }
        }
        
        if (leaving && Vector3.Distance(transform.position, destination) < 3.6f &&
            Vector3.Distance(transform.position, destination) > 3.55f)
        {
            m_gameManagerRef.ActivateDoors();
        }
    }
}
