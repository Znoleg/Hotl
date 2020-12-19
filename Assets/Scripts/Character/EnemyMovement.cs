using Pathfinding;
using System.Linq;
using UnityEngine;


public class EnemyMovement : NPCMovement
{
    private EnemyGraphics _enemyGraphics;
    private bool? _prevTrigger;

    protected override void Start()
    {
        base.Start();
        _enemyGraphics = GetComponent<EnemyGraphics>();
        DetectorCharacter.StatusChanged += OnStunStart;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HearCheck();

        var playerTransfrom = PlayerCharacter.Instance.transform;
        if (collision.transform == FireSound.Instance.transform)
        {
            AdditionalTrigger = true;
            AIDestinationSetter.target = playerTransfrom;
        }
    }

    private void OnStunStart(NPCStatus status)
    {
        if (status == NPCStatus.Stunned)
        {
            _prevTrigger = AdditionalTrigger;
            AdditionalTrigger = false;

            AiPath.enabled = false;
        }
        else if (AiPath.enabled == false)
        {
            OnStunEnd();
        }
    }

    private void OnStunEnd()
    {
        AdditionalTrigger = _prevTrigger;
        AiPath.enabled = true;
    }

    private void OnDestroy()
    {
        DetectorCharacter.StatusChanged -= OnStunStart;
        _enemyGraphics.StunSetter.OnStunEnd -= OnStunEnd;
    }
}


/// <summary>
/// Uses previous <see cref="DetectorMovement"/> stuff
/// <para>Able to walk on custom path defined in inspector</para>
/// <para>Also adds SoundHear mechanic</para>
/// </summary>
public abstract class NPCMovement : DetectorMovement
{
    #region fields

    [SerializeField] private Transform[] _defaultPath;
    [SerializeField] private bool _canHear = true;
    private int _currentPathPoint = 0;
    private float _maxSpeed;
    private float _shootDistance;
    private float _slowdownDistance;

    #endregion fields 

    protected bool CanHear => _canHear;

    #region Methods

    #region public Methods

    public void UpdateAiPath(Gun gun)
    {
        if (gun is null) return;
        if (gun is MeleeGun meleeGun)
        {
            var slowStopDiff = AiPath.endReachedDistance - AiPath.slowdownDistance;
            var hitRange = meleeGun.HitRange;
            if (hitRange < 1.85f) AiPath.endReachedDistance = 1.85f;
            else AiPath.endReachedDistance = hitRange;
            AiPath.slowdownDistance = AiPath.endReachedDistance - slowStopDiff;
        }
        else if (gun is ProjectileGun)
        {
            AiPath.endReachedDistance = _shootDistance;
            AiPath.slowdownDistance = _slowdownDistance;
        }
    }

    #endregion

    #region protected Methods

    protected override void Start()
    {
        base.Start();
        _maxSpeed = AiPath.maxSpeed;
        _shootDistance = AiPath.endReachedDistance;
        _slowdownDistance = AiPath.slowdownDistance;
    }

    protected override void Update()
    {
        base.Update();
        if (!IsTriggered && _defaultPath.Length > 0) MoveOnDefaultPath();
        AiPath.canSearch = IsTriggered;
        moveVelocity = AiPath.velocity;
    }

    protected override void Move()
    {
        //movement handled by AIPath
        RotateToTarget();
    }

    protected override bool CheckTrigger()
    {
        if (AdditionalTrigger.HasValue) return AdditionalTrigger.Value;
        return base.CheckTrigger();
    }

    protected void HearCheck()
    {
        if (!CanHear) return;
        if (TimelineManager.Instance && TimelineManager.Instance.IsPlaying) return;
    }

    #endregion

    #region private Methods

    private void RotateToTarget()
    {
        //var rotateTarget = notDestSetterTarget == null ? AIDestinationSetter.target : notDestSetterTarget;
        //Vector3 diff = rotateTarget.position - transform.position;
        //float rotationZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        Vector2 lookVector;
        if (DetectorCharacter.CanSeeTarget)
        {
            lookVector = Target.position - transform.position;
        }
        else
        {
            lookVector = AiPath.desiredVelocity;
        }
        float rotationZ = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    private void MoveOnDefaultPath()
    {
        var currentPoint = _defaultPath[_currentPathPoint];
        if (Vector2.Distance(transform.position, currentPoint.position) <= 1e-6)
        {
            if (_currentPathPoint == _defaultPath.Length - 1) _currentPathPoint = 0;
            else _currentPathPoint++;
        }
        //Vector3 dir = _defaultPath[_currentPathPoint].position - transform.position;
        //Vector3 translation = dir * _maxSpeed * Time.deltaTime;
        //transform.Translate(new Vector3(translation.x, translation.y, 0f));
        var step = _maxSpeed * Time.deltaTime;
        moveVelocity = Vector2.MoveTowards(transform.position, currentPoint.position, step);
        transform.position = moveVelocity;
        RotateToTarget();
    }
    #endregion

    #endregion Methods

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HearCheck();

        var walkSound = collision.transform.GetComponent<WalkSound>();

        if (walkSound) // can be Optimized
        {
            if ((this is EnemyMovement) && walkSound.GetComponentInParent<EnemyMovement>()) return;
            if ((this is FriendlyMovement) && walkSound.GetComponentInParent<FriendlyMovement>()) return;
            AdditionalTrigger = true;
            AIDestinationSetter.target = collision.GetComponentInParent<Character>().transform;
        }
    }
}


/// <summary>
/// Uses TransformDetector logic for CheckTrigger() and AIPath to walk
/// <para>Creates empty object to walk to when target ran away and destroys it when script object got to empty object position</para>
/// </summary>
public abstract class DetectorMovement : ObjectMovement
{
    [SerializeField] private bool _triggeredFromStart = false;
    [SerializeField] private GameObject _emptyPf;
    private GameObject _emptyCreated;
    private float _aiPathSpeed;

    public bool? AdditionalTrigger { get; set; }
    public AIPath AiPath { get; private set; }
    protected TransformDetector TransformDetector { get; private set; }
    public Transform Target => AIDestinationSetter.target.transform;
    public AIDestinationSetter AIDestinationSetter { get; private set; }
    protected DetectorCharacter DetectorCharacter { get; private set; }
    public override bool IsMoving => base.IsMoving && DetectorCharacter.CurrentStatus != NPCStatus.Stunned;


    protected override void Start()
    {
        base.Start();
        AiPath = GetComponent<AIPath>();
        DetectorCharacter = GetComponent<DetectorCharacter>();
        TransformDetector = new TransformDetector(DetectorCharacter);
        AIDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiPathSpeed = AiPath.maxSpeed;
        AiPath.canSearch = false;
        if (_triggeredFromStart)
        {
            AdditionalTrigger = _triggeredFromStart;
            if (AIDestinationSetter.target is null) AIDestinationSetter.target = DetectorCharacter.DetectingTargets.First();
        }


        //Utils.WaitCoroutine(() => DetectorCharacter.CharacterGraphics.StunSetter.OnStunEnd += OnStunEnd, 0.1f);
    }

    protected override bool CheckTrigger()
    {
        if (TimelineManager.Instance && TimelineManager.Instance.IsPlaying) return false;

        if (DetectorCharacter.CurrentStatus == NPCStatus.Stunned) return false;
        if (AdditionalTrigger.HasValue) return AdditionalTrigger.Value;

        var detectedTargets = TransformDetector.Detect().ToArray();

        if (AIDestinationSetter.target && detectedTargets.Contains(AIDestinationSetter.target))
        {
            //NPCStatusChanger.ChangeStatus(_detercorCharacter, NPCStatus.Raged);
            return true;
        }

        if (detectedTargets.Any())
        {
            DetectorCharacter.ChangeStatus(NPCStatus.Raged);
            AIDestinationSetter.target = detectedTargets.First(); // closest
            DeleteEmptyCreated();
            return true;
        }

        if (AIDestinationSetter.target)
        {
            //move to latest pos
            DetectorCharacter.ChangeStatus(NPCStatus.TargetLost);
            if (_emptyCreated && AIDestinationSetter.target == _emptyCreated.transform)
            {

                if (AiPath.reachedDestination)
                {
                    AIDestinationSetter.target = null;
                    DeleteEmptyCreated();
                    DetectorCharacter.ChangeStatus(NPCStatus.ActiveSearching);
                    ///
                    //rotate
                    //npcstatus walk
                    ///
                    return false; //doshli
                }
                return true;
            }
            else
            {

                _emptyCreated = Instantiate(_emptyPf, AIDestinationSetter.target.position, Quaternion.identity);
                AIDestinationSetter.target = _emptyCreated.transform;
                return true;
            }

        }
        if (DetectorCharacter.CurrentStatus == NPCStatus.Raged) DetectorCharacter.ChangeStatus(NPCStatus.Walk);
        return false;
    }

    private void DeleteEmptyCreated()
    {
        if (_emptyCreated)
        {
            Destroy(_emptyCreated);
            _emptyCreated = null;
        }
    }

}