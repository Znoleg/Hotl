using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class FireSound : MonoBehaviour
{
    private CircleCollider2D _collider;
    private const float _zeroRange = 0f;
    public static FireSound Instance { get; private set; }
 
    void Start()
    {
        Instance = this;
        _collider = GetComponent<CircleCollider2D>();
        Utils.CheckColliderTrigger(_collider);
        _collider.radius = _zeroRange;
    }

    public void DoSound(float range)
    {
        _collider.radius = range;
        StartCoroutine(Utils.WaitCoroutine(() => _collider.radius = _zeroRange));
    }
}
