using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerStats m_baseStats, m_currentStats;
    public Transform m_boatPrefab;
    public float m_respawnTime;

    bool m_onBoat, m_dead;
    PlayerMovement m_movement;
    PlayerActions m_actions;
    PlayerStatusEffects m_statusEffects;
    Vector3 m_startPos;

    Animator m_anim;

    private void Awake()
    {
        m_onBoat = false;
        m_movement = GetComponent<PlayerMovement>();
        m_actions = GetComponent<PlayerActions>();
        m_statusEffects = GetComponent<PlayerStatusEffects>();
        m_anim = transform.Find("Model").GetComponent<Animator>();

        // FOR NOW, ALWAYS RESET PLAYER STATS TO DEFAULT
        m_currentStats.Reset(m_baseStats);
        m_startPos = transform.position;
    }

    void Update ()
    {
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
        Camera.main.GetComponent<CameraController>().SetToBoatMode();
        GetComponent<CharacterController>().enabled = false;
        m_movement.MountBoat();
        m_actions.MountBoat();
        m_onBoat = true;
        m_anim.SetBool("Pickup", false);
        m_anim.SetBool("Jumping", false);
        m_anim.SetBool("InBoat", true);

        Transform newBoat = Instantiate(m_boatPrefab, transform.position, transform.rotation);
        newBoat.GetComponent<BoatMovement>().Initialize(m_currentStats, this);
    }

    public void DismountBoat(Vector3 _velocity)
    {
        Camera.main.GetComponent<CameraController>().SetToPlayerMode();
        GetComponent<CharacterController>().enabled = true;
        m_movement.DismountBoat(_velocity);
        m_actions.DismountBoat();
        m_onBoat = false;
        m_anim.SetBool("InBoat", false);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); // Stand back up
    }

    public void OnPlayerHit(int _damage)
    {
        if(m_dead)
            return;

        Debug.Log("player has taken " + _damage + " damage");
        m_currentStats.Life -= _damage;
        if(m_currentStats.Life <= 0)
        {
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
}
