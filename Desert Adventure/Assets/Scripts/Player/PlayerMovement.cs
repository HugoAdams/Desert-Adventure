using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private CharacterController charControl;

    public float m_walkSpeed;
    public float m_jumpSpeed;
    public float m_gravity;
    private float m_gravityScale;
    public float m_smoothMoveTime;

    public float m_dashSpeed;
    public float m_dashTime;
    public float m_dashRechargeTime;

    public float m_fallMultiplier = 2.5f; // Make gravity stronger when the player is falling
    public float m_lowJumpMultiplier = 2.0f; // Used to make the jump smaller if the player only taps the key

    private Vector3 currentMove = Vector3.zero;
    Vector3 m_refMove;

    private bool m_playerStunned, m_onBoat, m_dashRecharging, m_isAttacking;

    private PlayerStatusEffects m_playerStatusEffects;

    private Animator m_animator;

    void Awake () {
        charControl = GetComponent<CharacterController>();

        m_playerStatusEffects = GetComponent<PlayerStatusEffects>();
        m_playerStatusEffects.m_onStunned += onStunned;
        m_playerStatusEffects.m_onUnStunned += onUnStunned;

        m_animator = transform.Find("Model").GetComponent<Animator>();

        m_onBoat = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (!m_onBoat) // Can't do shiz if on boat
        {
            MovePlayer();
            JumpPlayer();
            DashPlayer();
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
            charControl.Move(currentMove * Time.fixedDeltaTime);
    }

    void DashPlayer()
    {
        if (m_dashRecharging || m_playerStunned || !charControl.isGrounded || m_isAttacking)
            return;
        if (Input.GetButtonDown("Dash"))
        {
            StartCoroutine(DashMove());
        }
    }

    IEnumerator DashMove()
    {
        m_animator.SetTrigger("DashedLeft");
        m_playerStunned = true;
        m_playerStatusEffects.onIncapacited();
        float timeStamp = Time.time + m_dashTime;

        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        while (timeStamp > Time.time)
        {
            // Rotated move dir by camera dir
            Vector3 NextDir = (Camera.main.transform.rotation * moveDirection).normalized;
            NextDir.y = 0;
            NextDir = NextDir * m_dashSpeed;
            NextDir.y = currentMove.y;
            currentMove = Vector3.SmoothDamp(currentMove, NextDir, ref m_refMove, m_smoothMoveTime); // Add movement to the move direction vector
            yield return null;
        }
        StartCoroutine(ResetDash());
        m_playerStunned = false;
        m_playerStatusEffects.onUnIncapacited();
        yield return null;
    }

    IEnumerator ResetDash()
    {
        m_dashRecharging = true;
        yield return new WaitForSeconds(m_dashRechargeTime);
        m_dashRecharging = false ;
        yield return null;
    }

    void JumpPlayer()
    {
        if (!m_playerStunned && !m_isAttacking && charControl.isGrounded && Input.GetButtonDown("Jump"))
        {
            currentMove.y = m_jumpSpeed; // Apply the jump speed if space bar hit
            m_animator.SetBool("Jumping", true);
        }
        else if (charControl.isGrounded)
        {
            m_animator.SetBool("Jumping", false);
            return;
        }

        if (currentMove.y < 0) // If the player is falling, make gravity stronger
            m_gravityScale = m_fallMultiplier;
        else if (!Input.GetButton("Jump")) // If the player is not falling and they are not holding the jump key, make the jump smaller by increasing gravity
            m_gravityScale = m_lowJumpMultiplier;
        else
            m_gravityScale = 1.0f;

        currentMove.y -= m_gravity * m_gravityScale * Time.deltaTime;
    }

    void MovePlayer()
    {
        Vector3 NextDir = new Vector3(0, 0, 0);
        if (!m_playerStunned && !m_isAttacking)
        {
            NextDir = (Camera.main.transform.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))).normalized;
            NextDir.y = 0;
            // Rotated move dir by camera dir
            if (NextDir != Vector3.zero) // Make the player look at the move direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(NextDir), 400 * Time.deltaTime);
        }
        NextDir = NextDir * m_walkSpeed;
        NextDir.y = currentMove.y;
        currentMove = Vector3.SmoothDamp(currentMove, NextDir, ref m_refMove, m_smoothMoveTime); // Add movement to the move direction vector
        Vector2 currentSpeed = new Vector2(charControl.velocity.x, charControl.velocity.z);
        m_animator.SetFloat("MoveSpeed", currentSpeed.sqrMagnitude);
    }

    void onStunned()
    {
        m_playerStunned = true;
        currentMove = new Vector3(0,0,0);
    }

    void onUnStunned()
    {
        m_playerStunned = false;
    }

    public void SetIsAttacking(bool isAttacking)
    {
        m_isAttacking = isAttacking;
    }
}
