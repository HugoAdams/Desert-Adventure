using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundClip
{
    public AudioClip m_audioClip;
    public string m_name;
}

public class SoundEffectsPlayer : MonoBehaviour {

    public SoundClip[] m_initalClips;
    Dictionary<string, AudioClip> m_soundDict;

    public static SoundEffectsPlayer Instance
    { get; private set; }

    AudioSource m_source;

    private void Awake()
    {
        // Shitty singleton baby
        Instance = this;
        m_source = GetComponent<AudioSource>();

        // Initialize soundDict
        m_soundDict = new Dictionary<string, AudioClip>();
        foreach (SoundClip sc in m_initalClips)
        {
            m_soundDict.Add(sc.m_name, sc.m_audioClip);
        }
    }

    public void PlaySound(string _name)
    {
        m_source.clip = m_soundDict[_name];
        m_source.PlayOneShot(m_source.clip);
    }
}
