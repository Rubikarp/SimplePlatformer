using UnityEngine.InputSystem;
using UnityEngine;
using System;

// Définition une liste d'état possible (marche, course, ou dans les airs)
public enum MovementState
{
    Walking,
    Running,
    Aerial
}

// Cet attribut rend obligatoire la présence d'un composant Rigidbody2D sur l'objet
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerState playerInfo;
    private Rigidbody2D body;

    [Header("Current State")]
    public MovementState currentState = MovementState.Walking;

    [Header("Movement Settings")]
    public MoveSettings walkSettings = new MoveSettings(10, 40, 40, 60);
    public MoveSettings runSettings = new MoveSettings(15, 60, 60, 40);
    public MoveSettings airSettings = new MoveSettings(8, 20, 20, 40);

    [Header("Calcul Interne")]
    [SerializeField] private float velocityX;
    [SerializeField] private float desiredvelocityX;

    [Header("Input Stick")]
    [SerializeField] private float horizontalInput;

    [Header("Input Sprint")]
    [SerializeField] private bool isPressingSprint;
    [SerializeField] private bool isSprintTooglable;

    private void Awake()
    {
        // Recherche du Rigidbody2D sur l'objet (sert à appliquer la physique : vitesse, gravité...)
        body = GetComponent<Rigidbody2D>();
        playerInfo.onLeaveGround.AddListener(OnLeaveGround);
        playerInfo.onTouchGround.AddListener(OnTouchGround);
    }

    private void OnLeaveGround()
    {
        currentState = MovementState.Aerial;
    }
    private void OnTouchGround()
    {
        if (isPressingSprint)
            currentState = MovementState.Running;
        else
            currentState = MovementState.Walking;
    }

    private void Update()
    {
        // Gère l'orientation du personnage (droite/gauche)
        UpdateLookDirection();
    }
    private void FixedUpdate()
    {
        // Calcul et applique la vélocité physique
        ComputeVelocity();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!playerInfo.CharacterCanMove)
        {
            horizontalInput = 0;
            return;
        }

        horizontalInput = context.ReadValue<Vector2>().x;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (isSprintTooglable)
        {
            if (context.started) isPressingSprint = !isPressingSprint;
        }
        else
        {
            if (context.started) isPressingSprint = true;
            if (context.canceled) isPressingSprint = false;
        }

        if (isPressingSprint && currentState == MovementState.Walking)
        {
            currentState = MovementState.Running;
        }
    }

    private void UpdateLookDirection()
    {
        //Si le joueur ne touche pas au stick on fait rien
        if (horizontalInput == 0) return;

        if (horizontalInput < 0)
        {
            // On regarde à gauche : on retourne le joueur sur l'axe x
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // On regarde à droite : on retablit le joueur sur l'axe x
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    private void ComputeVelocity()
    {
        MoveSettings moveSettings = currentState switch
        {
            MovementState.Walking => walkSettings,
            MovementState.Running => runSettings,
            MovementState.Aerial => airSettings,
            _ => walkSettings
        };

        float maxSpeedChange = float.MaxValue;
        if (horizontalInput == 0)
        {
            // Quand le joueur relâche le stick, on freine
            maxSpeedChange = moveSettings.maxDecceleration * Time.deltaTime;
        }
        else
        {
            // Vérifie si le joueur change de direction par rapport à sa vitesse actuelle
            bool IsChangingMoveDirection = Mathf.Sign(transform.localScale.x) != Mathf.Sign(body.linearVelocity.x);

            if (IsChangingMoveDirection)
            {
                // Tourner sur place (changement brusque de direction)
                maxSpeedChange = moveSettings.maxTurnSpeed * Time.deltaTime;
            }
            else
            {
                // Accélération normale
                maxSpeedChange = moveSettings.maxAcceleration * Time.deltaTime;
            }
        }

        desiredvelocityX = horizontalInput * moveSettings.maxSpeed;
        // On approche progressivement la vitesse actuelle vers la vitesse désirée en respectant la limite de "maxSpeedChange"
        velocityX = Mathf.MoveTowards(velocityX, desiredvelocityX, maxSpeedChange);

        body.linearVelocityX = velocityX;
    }
}

// Classe permettant d'organiser les paramètres de déplacement pour les différents états (marche, course, air)
[System.Serializable]
public struct MoveSettings
{
    [SerializeField, Range(0f, 20f)] public float maxSpeed;
    [SerializeField, Range(0f, 100f)] public float maxAcceleration;
    [SerializeField, Range(0f, 100f)] public float maxDecceleration;
    [SerializeField, Range(0f, 100f)] public float maxTurnSpeed;

    public MoveSettings(float maxSpeed = 10f, float maxAcceleration = 52f, float maxDecceleration = 52f, float maxTurnSpeed = 80f)
    {
        this.maxSpeed = maxSpeed;
        this.maxAcceleration = maxAcceleration;
        this.maxDecceleration = maxDecceleration;
        this.maxTurnSpeed = maxTurnSpeed;
    }
}