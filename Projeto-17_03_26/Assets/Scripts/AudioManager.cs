using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource systemSource;
    private List<AudioSource> activeSources;

    #region Singelton
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            systemSource = GetComponent<AudioSource>();
            activeSources = new List<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region AudioControls

    public void Play(AudioClip clip, AudioSource source)
    {
        if (!activeSources.Contains(source))
            activeSources.Add(source);
        systemSource.Stop();
        systemSource.clip = clip;
        systemSource.Play();
    }

    public void PlayOneShot(AudioClip clip,  AudioSource source)
    {
        source.PlayOneShot(clip);
    }
    public void Stop(AudioSource source)
    {
        if(activeSources.Contains(source))
            source.Stop();
        activeSources.Remove(source);
    }
    public void Pause(AudioSource source)
    {
        if (activeSources.Contains(source))
             source.Pause();
    }
    public void Resume(AudioSource source)
    {
        if (activeSources.Contains(source)) 
            source.UnPause();
    }

    #endregion
}
