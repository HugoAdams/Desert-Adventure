﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private CharacterController charControl;

    public KeyCode m_jumpKey;

    public float m_walkSpeed;
    public float m_jumpSpeed = 8.0f;
    public float m_gravity = 20.0f;
    private float m_gravityScale = 1.0f;

    public float m_fallMultiplier = 2.5f; // Make gravity stronger when the player is falling
    public float m_lowJumpMultiplier = 2.0f; // Used to make the jump smaller if the player only taps the key

    private Vector3 moveDirection = Vector3.zero;

    private bool m_playerStunned, m_onBoat;

    void Awake () {
        charControl = GetComponent<CharacterController>();

        GetComponent<PlayerStatusEffects>().m_onStunned += onStunned;
        GetComponent<PlayerStatusEffects>().m_onUnStunned += onUnStunned;

        m_onBoat = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (!m_onBoat) // Can't do shiz if on boat
        {
            MovePlayer();
            JumpPlayer();
        }
    }

    public void MountBoat()
    {
        m_onBoat = true;
    }

    public void DismountBoat()
    {
        m_onBoat = false;
    }

    void FixedUpdate()
    {
        if (!m_onBoat)
            charControl.Move(moveDirection * Time.deltaTime);
    }

    void JumpPlayer()
    {
        if (!m_playerStunned && charControl.isGrounded && Input.GetKeyDown(m_jumpKey))
            moveDirection.y = m_jumpSpeed; // Apply the jump speed if space bar hit
        else if (charControl.isGrounded)
            return;

        if (moveDirection.y < 0) // If the player is falling, make gravity stronger
            m_gravityScale = m_fallMultiplier;
        else if (!Input.GetKey(m_jumpKey)) // If the player is not falling and they are not holding the jump key, make the jump smaller by increasing gravity
            m_gravityScale = m_lowJumpMultiplier;
        else
            m_gravityScale = 1.0f;

        moveDirection.y -= m_gravity * m_gravityScale * Time.deltaTime;
    }

    void MovePlayer()
    {
        if (m_playerStunned)
            return;

        // Rotated move dir by camera dir
        Vector3 NextDir = (Camera.main.transform.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))).normalized;
        NextDir.y = 0;
        if (NextDir != Vector3.zero) // Make the player look at the move direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(NextDir), 650 * Time.deltaTime);
        NextDir = NextDir * m_walkSpeed;
        moveDirection = new Vector3(NextDir.x, moveDirection.y, NextDir.z); // Add movement to the move direction vector
    }

    void onStunned()
    {
        m_playerStunned = true;
        moveDirection = new Vector3(0,0,0);
    }

    void onUnStunned()
    {
        m_playerStunned = false;
    }
}
