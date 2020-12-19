using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DetectorCharacter : Character, IDetectorUser
{ 
    [Space]
    [SerializeField] private float _lookRange = float.PositiveInfinity;
    [SerializeField] private float _lookAmplitude = 90f;
    [SerializeField] private List<Transform> _targets = new List<Transform>();
    private List<PhysicalBody> raycastHit = new List<PhysicalBody>();

    public DetectorMovement DetectorMovement { get; private set; }
    protected List<Transform> Targets { get => _targets;  set => _targets = value; }
    public Transform Transform => transform;
    public List<Transform> DetectingTargets => _targets;
    public float DetectingDistance => _lookRange;
    public float DetectingAmplitude => _lookAmplitude;
    
    public bool CanSeeTarget => raycastHit.SkipWhile(hit => hit.Transparent || hit.transform == transform).FirstOrDefault() 
        is PhysicalBody target && target.transform == DetectorMovement.Target;

    protected override void Start()
    {
        base.Start();
        DetectorMovement = GetComponent<DetectorMovement>();
    }
    protected override bool CheckAttack()
    {
        if (!base.CheckAttack()) return IsAttacking = false;

        var raycastRange = (CurrentGun is MeleeGun meleeGun) ? meleeGun.HitRange : float.PositiveInfinity;

        //need fixes here
        if (!Utils.RaycastObject(transform.position, DetectorMovement.Target.position - transform.position, float.PositiveInfinity, out raycastHit))
        {
            return IsAttacking = false;
        }
        
        foreach (var body in raycastHit)
        {
            if (body.transform == DetectorMovement.AIDestinationSetter.target.transform) return IsAttacking = true;
            if (body.Transparent) return IsAttacking = false;
        }
        return false;
    }

}
