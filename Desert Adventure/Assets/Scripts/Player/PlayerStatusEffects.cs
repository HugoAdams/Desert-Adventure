using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour {

    public float m_flattenedRecoveryTime;
    public float m_flattenedStunTime;
    public float m_flattenedIncapacitedTime;

    public float m_spearedStunTime;
    public float m_spearedIncapacitedTime;
    public float m_spearedKnockbackstrength;

    private Transform m_model;
    private bool m_isFlattened;

    // Player can't move
    public delegate void OnStunned();
    public delegate void OnUnStunned();
    public event OnStunned m_onStunned;
    public event OnUnStunned m_onUnStunned;

    // Player can't use actions
    public delegate void OnIncapacited();
    public delegate void OnUnIncapacited();
    public event OnStunned m_onIncapacited;
    public event OnUnStunned m_onUnIncapacited;

    private CharacterController m_characterController;

    private bool m_isDead;

    private void Awake()
    {
        m_model = transform.Find("Model");
        m_characterController = GetComponent<CharacterController>();
    }

    public void FlattenedAttack()
    {
        if (m_isDead)
            return;

        if (!m_isFlattened)
        {
            StartCoroutine(Flattened());
            StartCoroutine(Incapacited(m_flattenedIncapacitedTime));
            StartCoroutine(Stunned(m_flattenedStunTime));
        };
    }


    public void OnDeath()
    {
        m_onStunned();
        m_onIncapacited();
        m_isDead = true;
    }

    public void OnRespawn()
    {
        m_onUnStunned();
        m_onUnIncapacited();
        m_isDead = false;
    }

    public void SpearedAttack(Vector3 attackDirection)
    {
        if (m_isDead)
            return;
        StartCoroutine(Incapacited(m_spearedIncapacitedTime));
        StartCoroutine(Stunned(m_spearedStunTime));
        StartCoroutine(KnockBack(m_spearedStunTime, attackDirection));
    }

    //Move the character in the directoin for the desired amount of time
    IEnumerator KnockBack(float knockbackTime, Vector3 attackDirection)
    {
        float m_timeStamp = Time.time;
        while ((m_timeStamp + knockbackTime) >= Time.time)
        {
            // Make the players model wobble in size
            float scas = ((Mathf.Sin(Time.time * 16) + 1.0f) * 0.2f) + 0.8f;
            m_model.localScale = new Vector3(scas, scas, scas);

            // Move the player in the direction of the attack
            m_characterController.Move(attackDirection * m_spearedKnockbackstrength * Time.deltaTime);
            yield return null;
        }
        m_model.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        yield return null;
    }

    // Stop the player controlling the character for the desired amount of time
        IEnumerator Stunned(float stunnedTime)
    {
        m_onStunned();
        yield return new WaitForSeconds(stunnedTime);
        if (!m_isDead)
            m_onUnStunned();
        yield return null;
    }

    // Stop the player from using actions for the desired amount of time
    IEnumerator Incapacited(float incapacitedTime)
    {
        m_onIncapacited();
        yield return new WaitForSeconds(incapacitedTime);
        if (!m_isDead)
            m_onUnIncapacited();
        yield return null;
    }

    
    IEnumerator Flattened()
    {
        m_isFlattened = true;
        float m_flattenedTimeStamp = Time.time;
        while ((m_flattenedTimeStamp + m_flattenedRecoveryTime) >= Time.time)
        {
            float scas = ((Mathf.Sin(Time.time * 16) + 1.0f) * 0.2f) + 0.8f;
            m_model.localScale = new Vector3(scas, 0.1f, scas);
            yield return null;
        }

        m_model.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        m_isFlattened = false;
        yield return null;
    }

    public void onIncapacited()
    {
        m_onIncapacited();
    }

    public void onUnIncapacited()
    {
        m_onUnIncapacited();
    }
}
