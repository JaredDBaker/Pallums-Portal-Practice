using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public bool gameIsLost = false;

    public bool GameIsLost
    {
        get { return gameIsLost; }
        set { gameIsLost = value; }
    }

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;

    public AudioClip clipMusic;
    public AudioClip clipFX;
    public AudioClip clipLost;

    private AudioSource musicSrc;
    private AudioSource fxSrc;
    private AudioSource lostSrc;

    public Slider musicSlider;
    public Slider fxSlider;

    public float musicVolume;
    public float fxVolume;

    public void Awake()
    {        
        musicSrc = AddAudio(clipMusic, true, true, 1.0f);
        fxSrc = AddAudio(clipFX, false, false, 1.0f);
        lostSrc = AddAudio(clipLost, false, false, 1.0f);
    }

    private void Start()
    {
        musicVolume = Menu.musicVolume;
        fxVolume = Menu.fxVolume;

        musicSlider.value = musicVolume;
        fxSlider.value = fxVolume;

        musicSrc.Play();
    }

    void Update()
    {
        musicSrc.volume = musicVolume;
        fxSrc.volume = fxVolume;

        Menu.musicVolume = musicVolume;
        Menu.fxVolume = fxVolume;

        if (!gameIsLost && Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        Time.timeScale = 1f; // Freeze game
        GameIsPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze game
        GameIsPaused = true;
    }

    public void QuitToMenu()
    {
        if (PlayerPrefs.GetInt("score") > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", PlayerPrefs.GetInt("score"));
        }
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
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

    public void PlayLostSound()
    {
        lostSrc.Play();
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

}
