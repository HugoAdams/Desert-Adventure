﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    public float m_pickUpRangeRadius;
    public float m_forwardThrowStrength;
    public float m_upwardsThrowStrength;

    private bool m_attacking;
    private bool m_holdingObject;
    private bool m_onBoat;
    private GameObject m_pickup;

    private bool m_playerIncapacited;

    private CharacterController m_characterController;
    private Animator m_charAnimator;
    private PlayerMovement m_playerMovement;

    private List<GameObject> m_attackHitboxes;

    private BoxCollider m_attack1BC;
    private BoxCollider m_attack2BC;
    private SphereCollider m_attackSC;

    private void Awake()
    {
        GetComponent<PlayerStatusEffects>().m_onIncapacited += onIncapacited;
        GetComponent<PlayerStatusEffects>().m_onUnIncapacited += onUnIncapacited;
        m_characterController = GetComponent<CharacterController>();
        m_charAnimator = transform.Find("Model").GetComponent<Animator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        GameObject attackhitboxes = transform.Find("AttackHitboxes").gameObject;
        m_attack1BC = attackhitboxes.transform.GetChild(0).GetComponent<BoxCollider>();
        m_attack2BC = attackhitboxes.transform.GetChild(1).GetComponent<BoxCollider>();
        m_attackSC = attackhitboxes.transform.GetChild(2).GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerIncapacited || m_onBoat || m_attacking)
            return;

        if (Input.GetButtonDown("Attack"))
        {
            Attack();
        }
        else if (Input.GetButtonDown("PickUp"))
        {
            if (!m_holdingObject)
            {
                m_pickup = GetClosestPickup();
                if (m_pickup != null)
                {
                    m_pickup.transform.rotation = new Quaternion(0, m_pickup.transform.rotation.y, 0, 1);
                    m_pickup.transform.SetParent(transform);
                    m_pickup.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                    DisableRagdoll(m_pickup.GetComponent<Rigidbody>());
                    m_holdingObject = true;
                }
            }
            else
            {
                if (m_characterController.velocity.sqrMagnitude <= 10)
                {
                    dropObject();
                }
                else
                {
                    throwObject();
                }
            }
        }
    }

    void dropObject()
    {
        m_pickup.transform.position = transform.position + (transform.forward * 1.5f);
        detatchPickup();
    }

    void throwObject()
    {
        detatchPickup();
        Vector3 throwforce = transform.forward * m_forwardThrowStrength;
        throwforce.y = m_upwardsThrowStrength;
        m_pickup.GetComponent<Rigidbody>().AddForce(throwforce);
    }

    void detatchPickup()
    {
        EnableRagdoll(m_pickup.GetComponent<Rigidbody>());
        m_pickup.transform.SetParent(null);
        m_holdingObject = false;
    }

    void EnableRagdoll(Rigidbody rb)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }
    void DisableRagdoll(Rigidbody rb)
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
    }

    // Gets the closest pickup to the player in range
    GameObject GetClosestPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_pickUpRangeRadius, 1 << 11);

        if (hitColliders.Length == 0)
            return null;

        GameObject closestPickup = hitColliders[0].gameObject;
        Vector3 currentPos = transform.position;
        float minDist = 10000;

        for (int i = 1; i < hitColliders.Length; ++i)
        {
            float dist = Vector3.Distance(hitColliders[i].transform.position, currentPos);
            if (dist < minDist)
            {
                closestPickup = hitColliders[i].gameObject;
                minDist = dist;
            }
        }
        return closestPickup;
    }

    void onIncapacited()
    {
        m_playerIncapacited = true;
        if(m_holdingObject)
        {
            m_pickup.transform.position = transform.position + (transform.forward * 1.5f);
            detatchPickup();
        }
    }

    void onUnIncapacited()
    {
        m_playerIncapacited = false;
    }

    void Attack()
    {
        if (m_holdingObject)
        {
            dropObject();
        }
        m_playerMovement.SetIsAttacking(true);
        m_attacking = true;
        m_charAnimator.SetTrigger("Attack");
        StartCoroutine(AttackEnum());
    }

    IEnumerator AttackEnum()
    {
        bool m_attackLinedUp = false;
        bool hitboxOn = false;
        float timeframe = Time.time + 0.833f;
        float hitboxTime = Time.time + 0.2f;
        float turnOffHitBox = timeframe - 0.2f;
        yield return null;
        while (timeframe > Time.time)
        {
            if (!m_attackLinedUp && Input.GetButtonDown("Attack"))
            {
                m_charAnimator.SetBool("Attack2", true);
                m_attackLinedUp = true;
            }
            if(!hitboxOn && hitboxTime <= Time.time)
            {
                hitboxOn = true;
                m_attack1BC.enabled = true;
            }
            if (hitboxOn && turnOffHitBox <= Time.time)
            {
                hitboxOn = false;
                m_attack1BC.enabled = false;
            }
            yield return null;
        }
        if (m_attackLinedUp)
        {
            m_attackLinedUp = false;
            timeframe = Time.time + 1.0f;
            hitboxTime = Time.time + 0.25f;
            yield return null;
            m_charAnimator.SetBool("Attack2", false);
            while (timeframe > Time.time)
            {
                if (!m_attackLinedUp && Input.GetButtonDown("Attack"))
                {
                    m_attackLinedUp = true;
                    m_charAnimator.SetBool("Attack3", true);
                }
                if (!hitboxOn && hitboxTime <= Time.time)
                {
                    hitboxOn = true;
                    m_attack2BC.enabled = true;
                }
                if (hitboxOn && turnOffHitBox <= Time.time)
                {
                    hitboxOn = false;
                    m_attack2BC.enabled = false;
                }
                yield return null;
            }
        }

        if (m_attackLinedUp)
        {
            hitboxOn = false;
            timeframe = Time.time + 1.0f;
            hitboxTime = Time.time + 0.25f;
            yield return null;
            m_charAnimator.SetBool("Attack3", false);
            while (timeframe > Time.time)
            {
                if (!m_attackLinedUp && Input.GetButtonDown("Attack"))
                {
                    m_attackLinedUp = true;
                }
                if (!hitboxOn && hitboxTime <= Time.time)
                {
                    hitboxOn = true;
                    m_attackSC.enabled = true;
                }
                if (hitboxOn && turnOffHitBox <= Time.time)
                {
                    hitboxOn = false;
                    m_attackSC.enabled = false;
                }
                yield return null;
            }
        }

        m_playerMovement.SetIsAttacking(false);
        m_attacking = false;
        yield return null;
    }

    public void MountBoat() { m_onBoat = true; }
    public void DismountBoat() { m_onBoat = false; }
}
