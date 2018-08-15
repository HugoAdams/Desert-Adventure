using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicRotator : MonoBehaviour {

    public AudioClip[] m_musicClips;
    int m_index;
    AudioSource m_audioSource;
    float m_toChangeTrack;

	void Start ()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_index = Random.Range(0, m_musicClips.Length);
        m_audioSource.clip = m_musicClips[m_index];
        m_toChangeTrack = Time.time + m_musicClips[m_index].length;
        m_audioSource.Play();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time >= m_toChangeTrack)
        {
            m_audioSource.Stop();
            m_index = (m_index + 1) % m_musicClips.Length;
            m_audioSource.clip = m_musicClips[m_index];
            m_toChangeTrack = Time.time + m_musicClips[m_index].length;
            m_audioSource.Play();
        }
	}
}
