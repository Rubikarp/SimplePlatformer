using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

//Special kind of "if" to not take in account the code in not inside the editor
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerState : MonoBehaviour
{
    [Header("Collider Settings")]
    [SerializeField, Range(0F, .2f)] private float groundCheckDistance = 0.05f;

    [Header("Layer Masks")]
    [SerializeField] private ContactFilter2D castFilter = new ContactFilter2D();
    [SerializeField] private List<RaycastHit2D> groundCastResult = new List<RaycastHit2D>(8);

    [Header("Info")]
    public bool CharacterCanMove;
    private CapsuleCollider2D capsule;
    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            //Check if value change
            if (_isGrounded == value) return;

            _isGrounded = value;
            if (_isGrounded)
                onTouchGround?.Invoke();
            else
                onLeaveGround?.Invoke();
        }
    }
    private bool _isGrounded;

    [Header("Events")]
    public UnityEvent onTouchGround;
    public UnityEvent onLeaveGround;

    private void Awake()
    {
        // Recherche du CapsuleCollider2D sur l'objet
        capsule = GetComponent<CapsuleCollider2D>();
    }
    private void FixedUpdate()
    {
        capsule.Cast(Vector2.down, castFilter, groundCastResult, groundCheckDistance);
        IsGrounded = groundCastResult.Count > 0;
    }
    private void OnDrawGizmos()
    {
        if (capsule == null) capsule = GetComponent<CapsuleCollider2D>();

//Special kind of "if" to not take in account the code in not inside the editor
#if UNITY_EDITOR
        Color color = IsGrounded? Color.green : Color.red;
        using (new Handles.DrawingScope(color))
        {
            Handles.DrawWireCube(capsule.bounds.center + Vector3.down * groundCheckDistance, capsule.bounds.size);
        }
#endif
    }
}
