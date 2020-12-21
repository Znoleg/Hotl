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
    private float _maxRaycastRange;
    private bool _canSeeTarget;

    public DetectorMovement DetectorMovement { get; private set; }
    protected List<Transform> Targets { get => _targets;  set => _targets = value; }
    public Transform Transform => transform;
    public List<Transform> DetectingTargets => _targets;
    public float DetectingDistance => _lookRange;
    public float DetectingAmplitude => _lookAmplitude;
    
    public bool CanSeeTarget => _canSeeTarget = raycastHit.SkipWhile(hit => hit.Transparent || hit.transform == transform).FirstOrDefault() 
        is PhysicalBody target && target.transform == DetectorMovement.Target;

    protected override void Start()
    {
        base.Start();
        DetectorMovement = GetComponent<DetectorMovement>();
        _maxRaycastRange = Screen.width * 1.5f;
    }
    
    protected override bool CheckAttack()
    {
        if (!base.CheckAttack()) return IsAttacking = false;

        var range = (CurrentGun is MeleeGun meleeGun) ? meleeGun.HitRange : float.PositiveInfinity;

        Utils.RaycastObject(transform.position, DetectorMovement.Target.position - transform.position, _maxRaycastRange, out raycastHit);

        //need fixes here
        if (_canSeeTarget)
        {
            if ((DetectorMovement.Target.transform.position - transform.position).magnitude <= range)
            {
                return IsAttacking = true;
            }
        }

        return IsAttacking = false;
    }

}
