using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    public enum Limit
    {
        Straight, Positive, Negative
    }

    [SerializeField] private bool _openedFromStart = true;
    private HingeJoint2D _hingeJoint;
    private Door _door;
    private JointAngleLimits2D _openDoorLimits;
    private JointAngleLimits2D _closeDoorLimits;
    private JointAngleLimits2D _positiveDoorLimits;
    private JointAngleLimits2D _negativeDoorLimits;
    private bool _playerTouched; // sets on true when (Player touches Door && Door is opening) || Door is closing

    private const float _doorChangeSeconds = 0.35f;


    public bool CanHitEnemy 
    {
        get
        {
            return IsClosing || (IsOpening && _playerTouched);
        }
     }

    public bool IsOpening { get; private set; }
    public bool IsClosing { get; private set; }

    public JointAngleLimits2D CloseLimits => _closeDoorLimits;

    public float Angle => _hingeJoint.jointAngle;

    public bool IsUnlocked => _hingeJoint.limits.min == _openDoorLimits.min && _hingeJoint.limits.max == _openDoorLimits.max;

    public Vector2 ForceApplied => _hingeJoint.reactionForce;

    public void UnlockDoor()
    {
        _hingeJoint.limits = _openDoorLimits;
    }

    public void LockDoor(Limit limit)
    {
        StartCoroutine(FreezeLimits(limit));
        
        //OnHingeClose?.Invoke();
    }

    public void CloseDoorOnce()
    {
        LockDoor(Limit.Straight);
        StartCoroutine(Utils.WaitCoroutine(UnlockDoor, 1f));
    }

    private void Awake()
    {
        _hingeJoint = GetComponentInChildren<HingeJoint2D>();
        _openDoorLimits = _hingeJoint.limits;
        _closeDoorLimits = new JointAngleLimits2D { min = 0f, max = 0f };
        _positiveDoorLimits = new JointAngleLimits2D { min = _openDoorLimits.max, max = _openDoorLimits.max };
        _negativeDoorLimits = new JointAngleLimits2D { min = _openDoorLimits.min, max = _openDoorLimits.min };
    }

    private void Start()
    {
        if (!_openedFromStart) LockDoor(Limit.Straight);
        _door = GetComponentInChildren<Door>();
        _door.OnPlayerTouch += OnPlayerTouch;
    }

    private void OnDestroy()
    {
        _door.OnPlayerTouch -= OnPlayerTouch;
    }

    private IEnumerator FreezeLimits(Limit limit)
    {
        switch (limit)
        {
            case Limit.Straight:
                _hingeJoint.limits = _closeDoorLimits;
                IsClosing = true;
                yield return new WaitForSeconds(_doorChangeSeconds);
                IsClosing = false;
                break;
            case Limit.Negative:
                _hingeJoint.limits = _negativeDoorLimits;
                IsOpening = true;
                yield return new WaitForSeconds(_doorChangeSeconds);
                IsOpening = false;
                SetPlayerTouched(false);
                break;
            case Limit.Positive:
                _hingeJoint.limits = _positiveDoorLimits;
                IsOpening = true;
                yield return new WaitForSeconds(_doorChangeSeconds);
                IsOpening = false;
                SetPlayerTouched(false);
                break;
        }
    }

    private void OnPlayerTouch()
    {
        SetPlayerTouched(true);
    }

    private void SetPlayerTouched(bool status)
    {
        _playerTouched = status;
    }
}
