using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public  CharacterName m_characterName;
    public  int m_stage;
    private GameObject m_playerRef;
    public string m_favouriteRecipe;
    private void Start()
    {
        m_playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        transform.LookAt(m_playerRef.transform, Vector3.up);
        transform.eulerAngles = new Vector3(90.0f, transform.eulerAngles.y, 0.0f);
    }
}
