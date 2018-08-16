using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerStats m_baseStats, m_currentStats;
    public Transform m_boatPrefab;
    public float m_respawnTime;
    public float m_invunTime;
    public Transform m_poofParticles;

    bool m_onBoat, m_dead, m_invun;
    PlayerMovement m_movement;
    PlayerActions m_actions;
    PlayerStatusEffects m_statusEffects;
    Vector3 m_startPos;
    CharacterController m_charcontroller;

    Animator m_anim;

    [HideInInspector]
    public bool m_specialDontMove = false;

    private void Awake()
    {
        m_onBoat = false;
        m_movement = GetComponent<PlayerMovement>();
        m_actions = GetComponent<PlayerActions>();
        m_statusEffects = GetComponent<PlayerStatusEffects>();
        m_anim = transform.Find("Model").GetComponent<Animator>();
        m_charcontroller = GetComponent<CharacterController>();
        // FOR NOW, ALWAYS RESET PLAYER STATS TO DEFAULT
        m_currentStats.Reset(m_baseStats);
        m_startPos = transform.position;
    }

    void Update ()
    {
        if(m_specialDontMove == true)
        {
            return;
        }
        if (!m_onBoat)
        {
            GetOnBoatLogic();
        }
	}

    void GetOnBoatLogic()
    {
        if (Input.GetButtonDown("BoatMounting"))
        {
            // No boat base = can't get on boat!
            if (!m_currentStats.BoatBase)
            {
                EffectCanvas.Instance.InformText("MISSING: Boat Hull");
                return;
            }

            MountBoat();
        }
    }

    void MountBoat()
    {
        if (!m_charcontroller.isGrounded)
            return;
        Camera.main.GetComponent<CameraController>().SetToBoatMode();
        GetComponent<CharacterController>().enabled = false;
        m_movement.MountBoat();
        m_actions.MountBoat();
        m_onBoat = true;
        m_anim.SetBool("PickUp", false);
        m_anim.SetBool("Jumping", false);
        m_anim.SetBool("InBoat", true);

        SoundEffectsPlayer.Instance.PlaySound("Pop");
        Transform newBoat = Instantiate(m_boatPrefab, transform.position, transform.rotation);
        newBoat.GetComponent<BoatMovement>().Initialize(m_currentStats, this);
        Instantiate(m_poofParticles, transform.position, Quaternion.identity);
    }

    public void DismountBoat(Vector3 _velocity)
    {
        SoundEffectsPlayer.Instance.PlaySound("Pop");
        Camera.main.GetComponent<CameraController>().SetToPlayerMode();
        GetComponent<CharacterController>().enabled = true;
        if(m_specialDontMove)
        {
            _velocity = Vector3.zero;
        }
        m_movement.DismountBoat(_velocity);
        m_actions.DismountBoat();
        m_onBoat = false;
        m_anim.SetBool("InBoat", false);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); // Stand back up
        Instantiate(m_poofParticles, transform.position, Quaternion.identity);
    }

    public void OnPlayerHit(int _damage)
    {
        if(m_dead || m_invun)
            return;

        Debug.Log("player has taken " + _damage + " damage");
        m_currentStats.Life -= _damage;
        SoundEffectsPlayer.Instance.PlaySound("PlayerGotHit");
        m_invun = true;
        Invoke("StopInvunerability", m_invunTime);
        if (m_currentStats.Life <= 0)
        {
            SoundEffectsPlayer.Instance.PlaySound("PlayerDeath");
            m_dead = true;
            m_currentStats.Life = 0;
            Debug.Log("Player has died");
            StartCoroutine(OnDeath());
            EventsController.Instance.TriggerPlayerLifeChange();
            return;
        }
        m_anim.SetTrigger("OnHit");
        EventsController.Instance.TriggerPlayerLifeChange();
    }

    void StopInvunerability()
    {
        m_invun = false;
    }

    private IEnumerator OnDeath()
    {
        m_anim.SetTrigger("OnDeath");
        m_statusEffects.OnDeath();
        yield return new WaitForSeconds(m_respawnTime);
        m_currentStats.Life = m_baseStats.Life;
        transform.position = m_startPos;
        m_anim.SetTrigger("OnRespawn");
        m_statusEffects.OnRespawn();
        EventsController.Instance.TriggerPlayerLifeChange();
        m_dead = false;
        yield return null;
    }

    public void LoseAllBoatParts()
    {
        Debug.Log("dropped");
        m_currentStats.BoatBase = false;
        m_currentStats.BoatMast = false;
        m_currentStats.BoatSail = false;
        m_currentStats.BoatTiller = false;
        EventsController.Instance.TriggerBoatPieceObtained(BoatPiece.TILLER);
    }

    public void addHealth(int health)
    {
        if (m_currentStats.Life == m_baseStats.Life)
            return;

        if (m_currentStats.Life < m_baseStats.Life)
        {
        if ((m_currentStats.Life + health) > m_baseStats.Life)
            m_currentStats.Life = m_baseStats.Life;
        else
            m_currentStats.Life += health;
        }
        EventsController.Instance.TriggerPlayerLifeChange();
    }
}
