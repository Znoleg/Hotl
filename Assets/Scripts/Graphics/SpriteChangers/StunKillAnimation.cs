using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class StunKillAnimation : CharacterGraphicsSpriteChanger
{
    public bool IsWorking { get; private set; }
    
    private readonly PlayerGraphics _playerGraphics;
    private bool _screenDrag;
    private readonly CoroutineObject _killAnimRoutine;
    private readonly CoroutineObject _playSoundRoutine;
    private readonly List<StunKillSprites> _stunKillSprites = new List<StunKillSprites>();
    private StunKillSprites _currentInUse;
    
    #region constructors

    public StunKillAnimation(PlayerGraphics playerGraphics, StunStatus startStatus = StunStatus.Floor) : base(playerGraphics)
    {
        _playerGraphics = playerGraphics;
        _killAnimRoutine = new CoroutineObject(playerGraphics, StartKillAnim);
        _playSoundRoutine = new CoroutineObject(playerGraphics, PlayKillSound);
        _stunKillSprites = _playerGraphics.StunKillSprites;

        ChangeKillSprites(startStatus);

        SwipeDetector.OnSwipe += GetSwipe;
    }


    public static SpriteChangerBuilder<StunKillAnimation> CreateBuilder(PlayerGraphics characterGraphics)
        => new SpriteChangerBuilder<StunKillAnimation>(characterGraphics);

    #endregion

    public event Action OnStunKillStart;
    public event Action OnAnimStart;
    public event Action OnStunKillEnd;

    public void StartKill(EnemyGraphics enemyGraphics)
    {
        ChangeKillSprites(enemyGraphics.StunSetter.Status); // player kill sprites type same as enemy death sprites type

        SetSprite(_currentInUse.StatusSprites[0]);
        OnStunKillStart?.Invoke();
        IsWorking = true;
        _killAnimRoutine.Start();
    }
   
    private void GetSwipe(SwipeData obj)
    {
        if (!IsWorking) return;
        if (obj.Direction == SwipeDirection.Right) _screenDrag = true;
    }

    private IEnumerator StartKillAnim() // starts kill anim after swipe
    {
        yield return new WaitUntil(() => _screenDrag);
        
        OnAnimStart?.Invoke();

        _playSoundRoutine.Start();
        Launch(cyclesAmount: 1, setFirstSpriteAfter: false, continueIfLaunched: true, onComplete: () => OnStunKillEnd.Invoke());
        
        _screenDrag = false;
        IsWorking = false;

        _killAnimRoutine.Stop();
        yield break;
    }

    private IEnumerator PlayKillSound()
    {
        yield return new WaitForSeconds(_currentInUse.SoundStartDelay);
        AudioManager.Instance.Play(_currentInUse.KillSound);
        
        _playSoundRoutine.Stop();
        yield break;
    }

    private void ChangeKillSprites(StunStatus status)
    {
        _currentInUse = _stunKillSprites.FindConfomity(status);
        Sprites = _currentInUse.StatusSprites;
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        SwipeDetector.OnSwipe -= GetSwipe;
    }
}

