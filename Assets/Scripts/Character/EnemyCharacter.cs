using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

public class EnemyCharacter : NPCCharacter, IDetectorUser
{
    private EnemyGraphics _enemyGraphics;
    private CoroutineObject _setColliderTrigger;
    private int _stdLayer;
    public EnemyMovement EnemyMovement { get; private set; }
    public EnemyGraphics EnemyGraphics => _enemyGraphics;

    public static event Action EnemyDied;

    public override void SetTargets()
    {
        var player = PlayerCharacter.Instance.transform;
        if (Targets.Contains(player) || DetectorMovement.AIDestinationSetter.target == player)
            throw new UnityException($"Multiple player target set in {gameObject.name} enemy!");
        /*if (Targets[0] == null) Targets[0] = player; else*/
        Targets.Add(player);

        var playerFriends = FindObjectsOfType<FriendlyCharacter>();
        foreach (var friend in playerFriends) if (!Targets.Contains(friend.transform)) Targets.Add(friend.transform);
    }

    //Pickup gun methods

    public override void TakeDamage(float amount)
    {
        //var character = LastHiter.GetComponent<Character>();
        //if (character && character.CurrentGun is MeleeGun meleeGun && meleeGun.TypeOfHit == MeleeGun.HitType.Stun)
        //    ChangeStatus(NPCStatus.Stunned);
        base.TakeDamage(amount);
    }

    protected override bool DestroyEvent()
    {
        EnemyDied?.Invoke();
        return base.DestroyEvent();
    }

    protected override void Awake()
    {
        base.Awake();
        _setColliderTrigger = new CoroutineObject(this, SetColliderTrigger);
    }

    protected override void Start()
    {
        base.Start();
        EnemyMovement = GetComponent<EnemyMovement>();

        StartCoroutine(Utils.WaitCoroutine(() =>
        {
            _enemyGraphics = GetComponent<EnemyGraphics>();
            Debug.Log(_enemyGraphics.StunSetter is null);
            _enemyGraphics.StunSetter.OnStunStart += OnStunStart;
            _enemyGraphics.StunSetter.OnStunEnd += OnStunEnd;
            _enemyGraphics.StunDeathAnimation.OnEnd += TakeDamage;
        }, 0.1f));
        
    }

    private void TakeDamage()
    {
        TakeDamage(100f);
    }

    private void OnStunStart()
    {
        _stdLayer = gameObject.layer;
        gameObject.layer = 15; // WallCollideOnly
        SpriteRenderer.sortingOrder--;
        //SetPhysicsActive(true, ColliderChangeMode.IsTrigger);
        _setColliderTrigger.Start();
    }

    private IEnumerator SetColliderTrigger()
    {
        yield return new WaitForSeconds(0.5f); // first we wait for character push start
        yield return new WaitUntil(() => !Rigitbody.IsFlying()); // then we wait for character push end
        SetPhysicsActive(true, ColliderChangeMode.IsTrigger);


        _setColliderTrigger.Stop();
        yield break;
    }

    private void OnStunEnd()
    {
        SetPhysicsActive(false, ColliderChangeMode.IsTrigger);
        SpriteRenderer.sortingOrder++;
        gameObject.layer = _stdLayer;
    }

    protected override void Update()
    {
        base.Update();
        if (DebugMode.Instance.LogEnemyStatus) Debug.Log(CurrentStatus);
    }

    private void OnDestroy()
    {
        _enemyGraphics.StunSetter.OnStunStart -= OnStunStart;
        _enemyGraphics.StunSetter.OnStunEnd -= OnStunEnd;
        _enemyGraphics.StunDeathAnimation.OnEnd -= TakeDamage;
    }
}
