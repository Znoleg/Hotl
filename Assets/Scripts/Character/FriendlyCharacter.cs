using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyCharacter : NPCCharacter, IDetectorUser
{
    public FriendlyMovement FriendlyMovement { get; private set; }
    
    public override void SetTargets()
    {
        var enemies = FindObjectsOfType<EnemyCharacter>();
        foreach (var enemy in enemies) if (!Targets.Contains(enemy.transform)) Targets.Add(enemy.transform);
    }

    protected override void Start()
    {
        base.Start();
        FriendlyMovement = GetComponent<FriendlyMovement>();
    }
}

public abstract class NPCCharacter : DetectorCharacter, IDetectorUser
{
    public NPCMovement NPCMovement { get; private set; }
    private CoroutineObject _initializeNPC;
    
    public abstract void SetTargets();

    protected override void Start()
    {
        base.Start();
        NPCMovement = GetComponent<NPCMovement>();
        _initializeNPC.Start();
        //if (!Movement.DestinationSetter) Movement.Init();
        
    }

    protected virtual void Awake()
    {
        _initializeNPC = new CoroutineObject(this, () => Utils.WaitCoroutine(() =>
        {
            NPCMovement.UpdateAiPath(CurrentGun);
            SetTargets();
            _initializeNPC.Stop();
        }));
    }

    protected override void Attack()
    {
        if (CurrentStatus == NPCStatus.Stunned) return;
        base.Attack();
    }
}