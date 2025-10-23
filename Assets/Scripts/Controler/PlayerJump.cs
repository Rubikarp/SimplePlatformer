using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum JumpState
{
    Grounded,
    Jumping,
    Falling
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerState playerInfo;
    private Rigidbody2D body;

    [Header("Current State")]
    public JumpState currentState;

    [Header("Jump Settings")]
    [SerializeField, Range(2f, 5.5f)]
    private float jumpHeight = 3f;
    [SerializeField, Range(0.2f, 1.25f)]
    private float timeToReachApex = 0.5f;

    [Header("Settings")]
    [SerializeField, Range(1f, 10f)]
    private float groundGravityMultiplier = 1f;
    [SerializeField, Range(0f, 5f)]
    private float jumpGravityMultiplier = .95f;
    [SerializeField, Range(1f, 10f)]
    private float fallGravityMultiplier = 1.75f;
    [Space]
    [SerializeField, Range(1f, 10f)]
    private float maxFallSpeed = 12f;

    [Header("Options Analogic")]
    public bool isJumpAnalogic = false;
    [SerializeField, Range(1f, 10f)]
    private float jumpCutOffGravityMultiplier = 1.75f;

    [Header("Options Coyotee Time")]
    public bool useCoyoteeTime = false;
    [SerializeField, Range(0f, 1f)]
    private float coyoteeTimeWindow = .2f;

    [Header("Options Jump Buffer")]
    public bool useJumpBuffer = false;
    [SerializeField, Range(0f, 1f)]
    private float jumpBufferTimeWindow = .2f;

    [Header("Info")]
    private bool isPressingJump;
    [Space]
    private float initialJumpForce;
    private float currentGravityMultiplier;
    private float leaveGroundMomment;
    private float lastJumpBufferMoment;

    [Header("Events")]
    public UnityEvent onJump;

    private void Awake()
    {
        // GetComponent() permet de rechercher sur son objet s'il y a un composant Rigidbody2D
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = groundGravityMultiplier;

        playerInfo.onLeaveGround.AddListener(OnLeaveGround);
        playerInfo.onTouchGround.AddListener(OnTouchGround);
    }

    private void FixedUpdate()
    {
        if (currentState == JumpState.Jumping && body.linearVelocity.y < 0) currentState = JumpState.Falling;

        var newGravityScale = GetGravityConstant();
        newGravityScale *= currentState switch
        {
            JumpState.Jumping => (isJumpAnalogic && !isPressingJump) ? jumpGravityMultiplier * jumpCutOffGravityMultiplier : jumpGravityMultiplier,
            JumpState.Falling => fallGravityMultiplier,
            JumpState.Grounded => groundGravityMultiplier,
            _ => groundGravityMultiplier,
        };
        //Gravité = Physics2D.gravity.y * gravityScale = gravityForce
        //Gravité = gravityForce / Physics2D.gravity.y
        //Et comme Physics2D.gravity.y est négatif, on l'inverse en mettant un - devant
        newGravityScale /= -Physics2D.gravity.y;
        currentGravityMultiplier = newGravityScale;
        body.gravityScale = currentGravityMultiplier;

        //Limite la velocité terminal (la vitesse de chute maximum)
        body.linearVelocityY = Mathf.Max(body.linearVelocityY, -maxFallSpeed);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!playerInfo.CharacterCanMove) return;

        if (context.started) TryToJump();
    }

    private void OnTouchGround()
    {
        if (useJumpBuffer)
        {
            bool inbufferWindow = (Time.time - lastJumpBufferMoment) < jumpBufferTimeWindow;
            if (inbufferWindow) Jump();
        }

        currentState = JumpState.Grounded;
    }
    private void OnLeaveGround()
    {
        leaveGroundMomment = Time.time;

        if (body.linearVelocity.y < 0f)
        {
            currentState = JumpState.Falling;
        }
        else
        {
            currentState = JumpState.Jumping;
        }
    }

    private void TryToJump()
    {
        if (!playerInfo.CharacterCanMove) return;

        bool validCoyoteeTime = useCoyoteeTime && (Time.time - leaveGroundMomment) < coyoteeTimeWindow;
        //Create the jump, provided we are on the ground, in coyote time, or have a double jump available
        if (!playerInfo.IsGrounded && !validCoyoteeTime)
        {
            //Put in Jump Buffer if activated
            if (useJumpBuffer) lastJumpBufferMoment = Time.time;
            return;
        }

        Jump();
    }

    private void Jump()
    {
        currentState = JumpState.Jumping;
        lastJumpBufferMoment = - ;

        // Compute initial velocity to reach apex at timeToApex:
        // InitialVelocity = GravityScale * TimeToReachApex
        initialJumpForce = GetGravityConstant() * timeToReachApex;
        body.linearVelocityY = initialJumpForce;

        onJump?.Invoke();
    }

    private float GetGravityConstant()
    {
        //Formule de physique :
        //Hauteur = (1/2) * Gravité * Temps²
        //Donc Gravité = (2 * Hauteur) / Temps²
        return (2f * jumpHeight) / Mathf.Pow(timeToReachApex, 2f);
    }

}