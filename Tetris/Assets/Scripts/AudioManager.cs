using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
    }

    #endregion

    public AudioSource music;
    public AudioSource gridClick;
    public AudioSource pieceDing;
    public AudioSource pieceThump;
    public AudioSource rotateClick;
    public AudioSource lineClear1;
    public AudioSource lineClear2;
    public AudioSource lineClear3;
    public AudioSource lineClear4;
    public AudioSource lineClear4Add;
    public AudioSource timerClick;
    public AudioSource timerStart;

    public AudioMixer SFXMixer;
    public AudioMixer MusicMixer;

    public Slider sfxSlider;
    public Slider musicSlider;


    private void Start()
    {
        float newVolume = 0f;
        SFXMixer.GetFloat("SFXVolume", out newVolume);
        newVolume /= 20;
        newVolume = Mathf.Pow(10, newVolume);
        sfxSlider.value = 1 - Mathf.Log10(newVolume) * 20;

        MusicMixer.GetFloat("MusicVolume", out newVolume);
        newVolume /= 20;
        newVolume = Mathf.Pow(10, newVolume);
    }
    public void SetMusicVol(float newVolume)
    {
        MusicMixer.SetFloat("MusicVolume", Mathf.Log10(newVolume) * 20);
    }

    public void SetSFXVol(float newVolume)
    {
        SFXMixer.SetFloat("SFXVolume", Mathf.Log10(newVolume) * 20);
    }

    public void PlayAudio(AudioSource audio)
    {
        audio.PlayOneShot(audio.clip);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeTo(music, 1f, .35f));
    }
    public void FadeOut()
    {
        StartCoroutine(FadeTo(music, 1f, 0f));
    }

    public void FadeThin()
    {
        StartCoroutine(FadeTo(music, .5f, 0.05f));
    }

    public void StartMusic()
    {
        music.Play();
    }

    IEnumerator FadeTo(AudioSource audio, float fadeTime, float fadeTo)
    {
        float iterationAmount = fadeTime / 80f;

        if(audio.volume > fadeTo)
        {
            while (audio.volume > fadeTo)
            {
                audio.volume -= iterationAmount;
                yield return new WaitForSecondsRealtime(iterationAmount);
            }
        }
        else
        {
            while (audio.volume < fadeTo)
            {
                audio.volume += iterationAmount;
                yield return new WaitForSecondsRealtime(iterationAmount);
            }
        }
        
    }

    
}
