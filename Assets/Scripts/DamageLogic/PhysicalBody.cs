using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PhysicalBody : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private bool transparent;

    public Collider2D Collider { get; private set; }

    public virtual bool Transparent { get; protected set; }

    protected virtual void Start()
    {
        _transform = transform;
        Collider = GetComponent<Collider2D>();
        if (Collider.isTrigger)
            throw new UnityException($"Object {gameObject.name} collider must NOT be trigger!");
    }

    
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }
}
