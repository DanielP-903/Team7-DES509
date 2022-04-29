using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.QuantumDialogue;
using Random = UnityEngine.Random;

[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> { }

public class Character : MonoBehaviour
{
    public SO_Character characterScriptableObject;
    public int stage = 0;
    [SerializeField]
    public StringIntDictionary CharacterStages;
    public IDictionary<string, int> StringIntDictionary
    {
        get { return CharacterStages; }
        set { CharacterStages.CopyFrom(value); }
    }

    private GameObject m_playerRef;
    private GameManager m_gameManagerRef;
    [HideInInspector] public bool isAvailable;
    [HideInInspector] public bool leaving;
    [HideInInspector] public QD_Dialogue currentDialogue;

    private bool m_characterWait = false;
    private float m_turnTimer = 0.0f;
    private void Awake()
    {
        m_gameManagerRef = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        m_playerRef = GameObject.FindGameObjectWithTag("Player");
        CharacterStages.Add("Shylo", 0);
        CharacterStages.Add("Docorty", 0);
        CharacterStages.Add("Mimi", 0);
    }

    private void Start()
    {
        isAvailable = false;
        leaving = false;
        transform.position = m_gameManagerRef.m_entryPos;

    }

    public void ChangeCharacter(SO_Character newCharacter)
    {
        characterScriptableObject = newCharacter;
        GetComponent<MeshRenderer>().sharedMaterial = characterScriptableObject.material;
        transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = characterScriptableObject.backMaterial;
        isAvailable = false;
        transform.position = m_gameManagerRef.m_entryPos;
    }

    public void SetExpression(Material mat)
    {
        GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    private void Update()
    {
        m_turnTimer = m_turnTimer <= 0 ? 0 : m_turnTimer - Time.deltaTime;

        GetComponent<BoxCollider>().enabled = isAvailable;

        if (!leaving)
        {
            transform.LookAt(m_playerRef.transform, Vector3.up);
            transform.eulerAngles = new Vector3(90.0f, transform.eulerAngles.y, 0.0f);
        }
        else
        {
            if (m_turnTimer <= 0)
            {
                m_turnTimer = 10.0f;
                GetComponent<Animator>().Play("CharTurn");
            }
            //transform.LookAt(m_gameManagerRef.m_entryPos, Vector3.up);
        }


        if (!isAvailable && !m_characterWait)
        {
            MoveIntoPosition();
        }
    }

    private void RandomiseNextCharacter()
    {
        CharacterName newCharacterName = m_gameManagerRef.currentCharacterName;
        while (newCharacterName == m_gameManagerRef.currentCharacterName)
        {
            int random = Random.Range(0, 2);
            newCharacterName = m_gameManagerRef.m_characterList[random].characterName;
            characterScriptableObject = m_gameManagerRef.m_characterList[random];
        }

        m_gameManagerRef.currentCharacterName = newCharacterName;
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
                isAvailable = false; // Change to true/false to allow/disallow stopping after char exit
                RandomiseNextCharacter();
                ChangeCharacter(m_gameManagerRef.FindCharacter());
                currentDialogue = characterScriptableObject.dialogues[CharacterStages[m_gameManagerRef.currentCharacterName.ToString()]];
                StartCoroutine(CharacterEntryDelay(3.0f));
            }
            else
            {
                isAvailable = true;
            }
        }

        if (leaving)
        {
            if (Vector3.Distance(transform.position, destination) < 6.8f &&
                Vector3.Distance(transform.position, destination) > 6.75f)
            {
                m_gameManagerRef.ActivateDoors();
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, destination) < 5.8f &&
                Vector3.Distance(transform.position, destination) > 5.75f)
            {
                m_gameManagerRef.ActivateDoors();
            }
        }

        //if (Vector3.Distance(transform.position, m_gameManagerRef.doorObject.transform.position) < 0.5f)
        //{
        //    m_gameManagerRef.ActivateDoors();
        //    m_gameManagerRef.doorObject.GetComponent<Animator>().Play("DoorOpen");
        //}
    }

    private IEnumerator CharacterEntryDelay(float waitTime)
    {
        m_characterWait = true;
        yield return new WaitForSeconds(waitTime);
        m_characterWait = false;
    }
}
