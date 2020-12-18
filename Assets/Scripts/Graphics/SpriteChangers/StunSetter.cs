using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum StunStatus
{
    Floor, Wall
}

public class StunSetter : CharacterGraphicsSpriteChanger
{
    #region fields

    private bool _riseUp = false;
    private const float _timeInStun = 5f;
    private List<InStunSprites> _inStunSprites = new List<InStunSprites>();
    private InStunSprites _currentInUse;
    private readonly EnemyCharacter _enemyCharacter;

    private readonly CoroutineObject _stunRoutine;
    private readonly CoroutineObject _tryChangeStatus;
    private readonly CoroutineObject _initRoutine;
    #endregion

    #region constructors

    public StunSetter(EnemyGraphics characterGraphics, StunStatus startStatus = StunStatus.Floor) : base(characterGraphics)
    {
        _enemyCharacter = characterGraphics.HolderEnemy;

        _inStunSprites = characterGraphics.GraphicSprites.InStunSprites;
        
        _stunRoutine = new CoroutineObject(_characterGraphics, InStun);
        _tryChangeStatus = new CoroutineObject(_characterGraphics, TryChangeStatus);
        _initRoutine = new CoroutineObject(_characterGraphics, InitRoutine);

        _currentInUse = _inStunSprites.FindConfomity(startStatus);
        Sprites = _currentInUse.StatusSprites;

        _initRoutine.Start();
    }

    public static SpriteChangerBuilder<StunSetter> CreateBuilder(EnemyGraphics characterGraphics)
        => new SpriteChangerBuilder<StunSetter>(characterGraphics);

    #endregion

    public StunStatus Status { get; private set; } = StunStatus.Floor;
    public bool IsWorking { get; private set; }

    public event Action OnStunStart;
    public event Action OnStunEnd;
    public event Action<StunStatus> OnStatusChange;

    #region methods

    public override void Deinitialize()
    {
        base.Deinitialize();
        _enemyCharacter.StatusChanged -= SetStun;
    }

    private void SetStun(NPCStatus status)
    {
        if (status != NPCStatus.Stunned) return;

        StartStun();
    }

    private void StartStun()
    {
        IsWorking = true;
        SetSprite(Sprites[0]);
        _enemyCharacter.Push(500f, _enemyCharacter.DirectionToFall);
        OnStunStart?.Invoke();
        _tryChangeStatus.Start();
        _stunRoutine.Start(); //inStun
    }

    private IEnumerator InStun()
    {
        _riseUp = false;
        yield return new WaitForSeconds(_timeInStun);
        yield return new WaitUntil(() => PlayerCharacter.Instance.PlayerGraphics.StunKillAnimation.IsWorking == false);

        if (_enemyCharacter.EnemyGraphics.StunDeathAnimation.IsWorking) yield break;

        _riseUp = true;
        ///
        Launch(cyclesAmount: 1);
        ///
        _enemyCharacter.ChangeStatus(NPCStatus.ActiveSearching);
        IsWorking = false;
        OnStunEnd?.Invoke();
    }

    private IEnumerator TryChangeStatus()
    {
        List<Collider2D> touchColliders = new List<Collider2D>();
        Rigidbody2D charBody = _enemyCharacter.Rigitbody;
        yield return new WaitUntil(() => charBody.IsFlying());
        while (charBody.IsFlying())
        {

            ContactFilter2D contactFilter2D = new ContactFilter2D();
            if (_enemyCharacter.Collider.OverlapCollider(contactFilter2D, results: touchColliders) > 0)
            { 
                 if (touchColliders.Any(coll => coll.GetComponent<TilemapCollider2D>())) // found wall
                {
                    ChangeStunStatus(StunStatus.Wall);
                }
            }
            yield return null;
        }
        
        _tryChangeStatus.Stop();
        yield break;
    }

    private IEnumerator InitRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        var characterGraphic = _enemyCharacter.CharacterGraphics.GraphicSprites;
        _enemyCharacter.StatusChanged += SetStun;
        _initRoutine.Stop();
        yield break;
    }

    private void ChangeStunStatus(StunStatus status)
    {
        Status = status;
        OnStatusChange?.Invoke(Status);

        ChangeStunSprites();
    }

    private void ChangeStunSprites() // сделать соответсвие enum/sprites
    {
        _currentInUse = _inStunSprites.FindConfomity(Status);
        Sprites = _currentInUse.StatusSprites;

        SetSprite(Sprites[0]);
    }

    #endregion
}