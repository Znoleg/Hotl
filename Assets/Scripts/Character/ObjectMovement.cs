using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Abstract class that provides interface for move logic
/// <para> Have CheckTrigger() abstract method that sets 'IsTriggered' property on necessary value </para>
/// <para> This Property causes Move() abstract method to workd by it's value </para>
/// 
/// </summary>
[RequireComponent(typeof(Character), typeof(Collider2D))]
public abstract class ObjectMovement : MonoBehaviour
{
    #region fields

    [SerializeField] protected float moveSpeed = 5f;
    protected Vector2 moveVelocity;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform _transform;

    #endregion fields

    public bool IsTriggered { get; 
        protected set; }
    public virtual bool IsMoving 
    { 
        get 
        {
            return !moveVelocity.IsZero();
        } 
    }

    protected virtual void Start()
    {
        _transform = transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //IsTriggered = true;
    }

    /// <summary>
    /// Called in FixedUpdate() if IsTriggered
    /// </summary>
    protected abstract void Move();
    protected abstract bool CheckTrigger();
    // Update is called once per frame
    protected virtual void Update()
    {
        IsTriggered = CheckTrigger();
    }

    protected virtual void FixedUpdate()
    {
        if (IsTriggered)
        {
            Move();
        }
    }

}
