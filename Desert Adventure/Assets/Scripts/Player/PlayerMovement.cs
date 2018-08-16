using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private CharacterController charControl;

    [Header("Movement vairables")]
    public float m_walkSpeed;
    public float m_jumpSpeed;
    public float m_gravity;
    private float m_gravityScale;
    public float m_smoothMoveTime;
    public float m_attackMoveSpeed = 20;

    [Header("Dash variables")]
    public float m_dashSpeed;
    public float m_dashTime;
    public float m_dashRechargeTime;

    [Header("Jump variables")]
    public float m_fallMultiplier = 2.5f; // Make gravity stronger when the player is falling
    public float m_lowJumpMultiplier = 2.0f; // Used to make the jump smaller if the player only taps the key

    [Header("Sliding variables")]
    public float m_startSlidingAngle = 45;
    public float m_stopSlidingAngle = 25;
    LayerMask m_groundLayer;
    float m_slideStrength;

    [Header("Walking Audio")]
    public float m_timeBetweenAudioSteps = 0.4f;
    public float m_speedThreshold = 0.5f;
    float m_currentAudioStepTime;
    AudioSource m_stepAudio;

    private Vector3 currentMove = Vector3.zero;
    Vector3 m_refMove;

    private bool m_playerStunned, m_onBoat, m_dashRecharging, m_isAttacking, m_sliding, m_jumping, m_Grounded;

    private PlayerStatusEffects m_playerStatusEffects;
    private Animator m_animator;
    private ParticleSystem m_flyingSandParticles;

    [HideInInspector]
    public bool m_specialDontMove = false;

    void Awake () {
        charControl = GetComponent<CharacterController>();

        m_playerStatusEffects = GetComponent<PlayerStatusEffects>();
        m_playerStatusEffects.m_onStunned += onStunned;
        m_playerStatusEffects.m_onUnStunned += onUnStunned;
        m_flyingSandParticles = transform.Find("FlyingSand").GetComponent<ParticleSystem>();
        m_animator = transform.Find("Model").GetComponent<Animator>();
        m_groundLayer = 1 << LayerMask.NameToLayer("Terrain");
        m_stepAudio = GetComponent<AudioSource>();

        m_onBoat = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (m_onBoat) // Can't do shiz if on boat
            return;

        CheckIfGrounded();

        if (m_specialDontMove)
        {
            MovePlayer();
            return;
        }

        if (!m_sliding)
        {
            MovePlayer();
            JumpPlayer();
            DashPlayer();
            CheckSlideLogic();
            StepAudioLogic();
        }
        else
        {
            SlidePlayer();
        }
    }

    void StepAudioLogic()
    {
        float mag = new Vector3(currentMove.x, 0, currentMove.z).sqrMagnitude;

        if (!charControl.isGrounded || mag < m_speedThreshold)
        {
            m_currentAudioStepTime = Time.time;
        }
        else
        {
            if (Time.time - m_currentAudioStepTime > m_timeBetweenAudioSteps && mag > m_speedThreshold)
            {
                m_stepAudio.PlayOneShot(m_stepAudio.clip);
                m_currentAudioStepTime = Time.time;
            }
        }
    }

    void CheckIfGrounded()
    {
        if(m_Grounded && !charControl.isGrounded)
        {
            m_flyingSandParticles.Pause();
            m_Grounded = false;
        }
        else if(!m_Grounded && charControl.isGrounded)
        {
            m_flyingSandParticles.Play();
            m_Grounded = true;
        }
    }

    void CheckSlideLogic()
    {
        if (!charControl.isGrounded)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5, m_groundLayer))
        {
            Vector3 normal = hit.normal;
            float floorAngle = Mathf.Acos(Mathf.Abs(Vector3.Dot(normal, Vector3.up))) * Mathf.Rad2Deg;
            //Debug.Log(floorAngle);

            if (floorAngle > m_startSlidingAngle)
                m_sliding = true;
        }
    }

    void SlidePlayer()
    {
        if (!charControl.isGrounded)
        {
            m_sliding = false;
            return;
        }

        m_slideStrength = Mathf.Clamp01(m_slideStrength + Time.deltaTime);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5, m_groundLayer))
        {
            Vector3 normal = hit.normal;
            float floorAngle = Mathf.Acos(Mathf.Abs(Vector3.Dot(normal, Vector3.up))) * Mathf.Rad2Deg;

            if (floorAngle <= m_stopSlidingAngle)
            {
                m_sliding = false;
                m_slideStrength = 0;
                return;
            }
            else
            {
                // Slide!
                Vector3 slideVel = Vector3.zero;
                slideVel.x += (1f - normal.y) * normal.x * (1f - 0.0f);
                slideVel.z += (1f - normal.y) * normal.z * (1f - 0.0f);
                slideVel = slideVel.normalized * m_walkSpeed * m_slideStrength;
                currentMove = new Vector3(slideVel.x, currentMove.y, slideVel.z);
            }
        }
    }

    public void MountBoat()
    {
        m_onBoat = true;
        m_flyingSandParticles.Stop();
    }

    public void DismountBoat(Vector3 _velocity)
    {
        m_flyingSandParticles.Play();
        m_onBoat = false;

        if(_velocity.sqrMagnitude > 300.0f)
            currentMove = _velocity * 0.3f;
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
            if (charControl.isGrounded)
            {
                // Rotated move dir by camera dir
                Vector3 NextDir = (Camera.main.transform.rotation * moveDirection).normalized;
                NextDir.y = 0;
                NextDir = NextDir * m_dashSpeed;
                NextDir.y = currentMove.y;
                currentMove = Vector3.SmoothDamp(currentMove, NextDir, ref m_refMove, m_smoothMoveTime); // Add movement to the move direction vector
            }
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
            SoundEffectsPlayer.Instance.PlaySound("Jump");
            currentMove.y = m_jumpSpeed; // Apply the jump speed if space bar hit
            m_animator.SetBool("Jumping", true);
            m_jumping = true;
        }
        else if (charControl.isGrounded)
        {
            m_animator.SetBool("Jumping", false);
            m_animator.SetBool("IsFalling", false);
            m_jumping = false;
                     return;
        }

        if (currentMove.y < 0) // If the player is falling, make gravity stronger
        {
            //m_animator.SetBool("IsFalling", true);
            m_gravityScale = m_fallMultiplier;
        }
        else if (!Input.GetButton("Jump")) // If the player is not falling and they are not holding the jump key, make the jump smaller by increasing gravity
            m_gravityScale = m_lowJumpMultiplier;
        else
            m_gravityScale = 1.0f;

        currentMove.y -= m_gravity * m_gravityScale * Time.deltaTime;
    }

    void MovePlayer()
    {
        if (m_isAttacking)
        {
            currentMove = Vector3.SmoothDamp(currentMove, Vector3.zero, ref m_refMove, m_smoothMoveTime * 0.8f); 
            return;
        }

        Vector3 NextDir = new Vector3(0, 0, 0);
        if (m_playerStunned == false && m_specialDontMove == false) 
        {
            NextDir = (Camera.main.transform.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))).normalized;
            NextDir.y = 0;
            // Rotated move dir by camera dir
            if (NextDir != Vector3.zero) // Make the player look at the move direction
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(NextDir), 800 * Time.deltaTime);
        }
        NextDir = NextDir * m_walkSpeed;
        NextDir.y = currentMove.y;
        float smoothAmount = (m_jumping || charControl.isGrounded) ? m_smoothMoveTime : m_smoothMoveTime * 10.0f;
        currentMove = Vector3.SmoothDamp(currentMove, NextDir, ref m_refMove, smoothAmount); // Add movement to the move direction vector
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

    public void SetIsAttacking(bool isAttacking, float _delayMove = 0)
    {
        m_isAttacking = isAttacking;

        if (isAttacking)
        {
            StartCoroutine(ApplyAttackMovespeed(_delayMove));
        }
    }

    IEnumerator ApplyAttackMovespeed(float _delay)
    {
        if (_delay > 0)
            yield return new WaitForSeconds(_delay);

        Vector3 faceDir = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        currentMove = faceDir * m_attackMoveSpeed;
    }
}
