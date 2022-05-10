using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject volSlider;
    public SO_Volume VolumeScaleMenu;
    public AudioSource bg_menu_music;

    void Start()
    {
        bg_menu_music.volume = VolumeScaleMenu.volume;
        volSlider.GetComponent<Slider>().value = VolumeScaleMenu.volume;
    }
    void Update()
    {
        VolumeScaleMenu.volume = volSlider.GetComponent<Slider>().value;
        bg_menu_music.volume = VolumeScaleMenu.volume;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
