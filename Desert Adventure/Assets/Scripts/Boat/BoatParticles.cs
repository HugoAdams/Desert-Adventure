using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Also checks if grounded lololol

public class BoatParticles : MonoBehaviour {

    public AudioSource m_windAudio;
    AudioSource m_audioSource;
    ParticleSystem[] m_particles;
    BoatMovement m_boat;
    Rigidbody m_rb;
    int m_counter;
    float m_maxVol;
    float m_maxWindVol;
    float m_dir;

    private void Awake()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
        ToggleParticles(false);
        m_boat = GetComponentInParent<BoatMovement>();
        m_rb = m_boat.GetComponent<Rigidbody>();
        m_audioSource = GetComponent<AudioSource>();
        m_maxVol = m_audioSource.volume;
        m_dir = -1;
        m_audioSource.volume = 0;
        m_maxWindVol = m_windAudio.volume;
        m_windAudio.volume = 0;
    }

    private void Start()
    {
        m_audioSource.Play();
        m_windAudio.Play();
    }

    private void Update()
    {
        float increment = 0.3f;
        float mag = new Vector3(m_rb.velocity.x, 0, m_rb.velocity.z).sqrMagnitude;

        if (m_counter > 0)
        {
            if (mag > 9)
                m_dir = 1;
            else if (mag < 9)
                m_dir = -1;
        }
        else
            m_dir = -1;

        m_audioSource.volume = Mathf.Clamp(m_audioSource.volume + m_dir * increment * Time.deltaTime, 0, m_maxVol);

        // Wind audio dependent on speed
        m_windAudio.volume = Mathf.Lerp(m_windAudio.volume, Mathf.Clamp(m_maxWindVol * (m_rb.velocity.sqrMagnitude / (m_boat.m_maxVelocity * m_boat.m_maxVelocity)), 0, m_maxWindVol), Time.deltaTime);
    }

    void ToggleParticles(bool _on)
    {
        foreach (ParticleSystem ps in m_particles)
        {
            if (_on)
                ps.Play();
            else
                ps.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // CAN ONLY COLLIDE WITH TERRAIN
        m_counter++;
        if (m_counter == 1)
        {
            ToggleParticles(true);
            m_boat.m_grounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_counter--;
        if (m_counter < 1)
        {
            ToggleParticles(false);
            m_boat.m_grounded = false;
        }
    }
}
