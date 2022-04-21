using System.Collections.Generic;
using UnityEngine;

public enum CharacterName
{
    Shylo, Mimi, Docorty
}

public class GameManager : MonoBehaviour
{
    public List<GameObject> m_ingredientList;
    public List<SO_Character> m_characterList;

    // Game Flags
    public static bool m_hasBrewedATea = false;

    public CharacterName currentCharacterName = CharacterName.Shylo;
    public Character currentCharacter;
    public Vector3 m_entryPos;
    public Vector3 m_exitPos;

    private float m_doorDelay;

    [SerializeField] private Animator m_doors;

    void OnDrawGizmos()
    {
        Color color = Color.yellow;
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawSphere(m_entryPos, 1);

        color = Color.green;
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawSphere(m_exitPos, 1);
    }

    void Start()
    {
        ActivateDoors();
        currentCharacter = GameObject.FindGameObjectWithTag("Character").GetComponent<Character>();
        currentCharacter.ChangeCharacter(FindCharacter());
        currentCharacter.currentDialogue = currentCharacter.characterScriptableObject.dialogues[currentCharacter.stage];
    }

    void Update()
    {
        m_doorDelay = m_doorDelay <= 0 ? 0 : m_doorDelay - Time.deltaTime;
    }

    public SO_Character FindCharacter()
    {
        foreach (var character in m_characterList)
        {
            if (character.characterName == currentCharacterName)
            {
                return character;
            }
        }

        if (m_characterList.Count == 0)
        {
            Debug.LogError("Character List in Game Manager is empty!");
            Debug.DebugBreak();
        }

        return null;
    }

    public void ActivateDoors()
    {
        if (m_doorDelay <= 0)
        {
            m_doors.SetTrigger("Open");
            m_doorDelay = 1.0f;
        }
    }
}
