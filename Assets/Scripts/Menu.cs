using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public AudioClip clipMusic;
    public AudioClip clipFX;

    private AudioSource musicSrc;
    private AudioSource fxSrc;

    public static float musicVolume = 1f;
    public static float fxVolume = 1f;

    public UnityEngine.UI.Image backgroundImage;

    public Sprite menuSprite;
    public Sprite optionsSprite;

    public Slider musicSlider;
    public Slider fxSlider;

    private bool optionsEnabled = false;
    public void Awake()
    {
        musicSrc = AddAudio(clipMusic, true, true, musicVolume);
        fxSrc = AddAudio(clipFX, false, false, fxVolume);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("score", 0);
        musicSrc.Play();
        musicSlider.value = musicVolume;
        fxSlider.value = fxVolume;
    }

    private void Update()
    {
        musicSrc.volume = musicVolume;
        fxSrc.volume = fxVolume;
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
    }

    public void SetFXVolume(float vol)
    {
        fxVolume = vol;
    }

    public void PlayClickingSound()
    {
        fxSrc.Play();
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();

        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;

        return newAudio;
    }

    public void ChangeBackground()
    {
        optionsEnabled = !optionsEnabled;
        if (optionsEnabled)
        {
            backgroundImage.sprite = optionsSprite;
        }
        else
        {
            backgroundImage.sprite = menuSprite;
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("Lore");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("TutTransition");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
