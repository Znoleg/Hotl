using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DetectorCharacter : Character, IDetectorUser
{ 
    [Space]
    [SerializeField] private float _lookRange = float.PositiveInfinity;
    [SerializeField] private float _lookAmplitude = 90f;
    [SerializeField] private List<Transform> _targets = new List<Transform>();
    private DetectorMovement _detectorMovement;

    public DetectorMovement DetectorMovement => _detectorMovement;

    protected List<Transform> Targets { get { return _targets; } set { _targets = value; } }

    public Transform Transform => transform;

    public List<Transform> DetectingTargets => _targets;

    public float DetectingDistance => _lookRange;

    public float DetectingAmplitude => _lookAmplitude;

    protected override void Start()
    {
        base.Start();
        _detectorMovement = GetComponent<DetectorMovement>();
    }
    protected override bool CheckAttack()
    {
        if (!base.CheckAttack()) return IsAttacking = false;

        var raycastRange = (CurrentGun is MeleeGun meleeGun) ? meleeGun.HitRange : float.PositiveInfinity;
        if (!Utils.RaycastObject<PhysicalBody>(transform.position, transform.right, raycastRange, out var physicalBodies))
            return IsAttacking = false;
        
        foreach (var body in physicalBodies)
        {
            if (body.transform == _detectorMovement.AIDestinationSetter.target.transform) return IsAttacking = true;
            if (body.Transparent) return IsAttacking = false;
        }
        return false;
    }

}
