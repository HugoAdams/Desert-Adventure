using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Also checks if grounded lololol

public class BoatParticles : MonoBehaviour {

    ParticleSystem[] m_particles;
    BoatMovement m_boat;
    int m_counter;

    private void Awake()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
        ToggleParticles(false);
        m_boat = GetComponentInParent<BoatMovement>();
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
