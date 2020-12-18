using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class WalkSound : MonoBehaviour
{
    private CircleCollider2D _collider;
    private ObjectMovement _movement;

    private void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        _movement = GetComponentInParent<ObjectMovement>();
        Utils.CheckColliderTrigger(_collider);
    }

    private void Update()
    {
        gameObject.SetActive(_movement.IsMoving);
    }
}
