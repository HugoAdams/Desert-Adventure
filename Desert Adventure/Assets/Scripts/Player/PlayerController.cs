using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerStats m_baseStats, m_currentStats;
    public Transform m_boatPrefab;

    bool m_onBoat;
    PlayerMovement m_movement;
    PlayerActions m_actions;
    Vector3 m_startPos;

    private void Awake()
    {
        m_onBoat = false;
        m_movement = GetComponent<PlayerMovement>();
        m_actions = GetComponent<PlayerActions>();

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

        Transform newBoat = Instantiate(m_boatPrefab, transform.position, transform.rotation);
        newBoat.GetComponent<BoatMovement>().Initialize(m_currentStats, this);
    }

    public void DismountBoat()
    {
        Camera.main.GetComponent<CameraController>().SetToPlayerMode();
        GetComponent<CharacterController>().enabled = true;
        m_movement.DismountBoat();
        m_actions.DismountBoat();
        m_onBoat = false;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); // Stand back up
    }

    public void OnPlayerHit(int _damage)
    {
        Debug.Log("player has taken " + _damage + " damage");
        m_currentStats.Life -= _damage;
        if(m_currentStats.Life <= 0)
        {
            m_currentStats.Life = 0;
            Debug.Log("Player has died");
            OnDeath();
            return;
        }
        EventsController.Instance.TriggerPlayerLifeChange();
    }

    private void OnDeath()
    {
        m_currentStats.Life = m_baseStats.Life;
        transform.position = m_startPos;
        EventsController.Instance.TriggerPlayerLifeChange();
    }
}
