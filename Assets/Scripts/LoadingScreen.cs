using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject m_loadingBar;
    [SerializeField]
    private float m_speed;

    // Update is called once per frame
    void Update()
    {
        m_loadingBar.GetComponent<Slider>().value =
            Mathf.Lerp(m_loadingBar.GetComponent<Slider>().value, 100.0f, Time.deltaTime * m_speed);

        if (m_loadingBar.GetComponent<Slider>().value > 99.0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
