using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : Singleton<BGMusic>
{
    // public static BGMusic Instance;

    public AudioSource m_audioSource;
    public AudioSource btn_audioSource;
    public AudioClip btnSoundClip;
    private void Start()
    {
        btn_audioSource.clip =btnSoundClip;
        StartCoroutine(BGMusicStart());
        /*  if (Instance != null)
          {
              DestroyImmediate(gameObject);
          }
          else
          {
              DontDestroyOnLoad(gameObject);
              Instance = this;
              m_audioSource = GetComponent<AudioSource>();
             // m_audioSource.ignoreListenerVolume = true;
              //m_audioSource.volume = PlayerPrefs.GetInt("music_on");
              //AudioListener.volume = PlayerPrefs.GetInt("sound_on");
          }*/
    }
    private IEnumerator BGMusicStart()
    { 
        yield return new WaitForSeconds(7);
        m_audioSource.enabled = true;
    }
public void FadeIn()
    {
        if (PlayerPrefs.GetInt("music_on") == 1)
        {
            StartCoroutine(FadeAudio(1.0f, Fade.In));
        }
    }

    public void FadeOut()
    {
        if (PlayerPrefs.GetInt("music_on") == 1)
        {
            StartCoroutine(FadeAudio(1.0f, Fade.Out));
        }
    }

    private enum Fade
    {
        In,
        Out
    }

    private IEnumerator FadeAudio(float time, Fade fadeType)
    {
        var start = fadeType == Fade.In ? 0.0f : 1.0f;
        var end = fadeType == Fade.In ? 1.0f : 0.0f;
        var i = 0.0f;
        var step = 1.0f / time;

        while (i <= 1.0f)
        {
            i += step * Time.deltaTime;
            m_audioSource.volume = Mathf.Lerp(start, end, i);
            yield return new WaitForSeconds(step * Time.deltaTime);
        }
    }
}
