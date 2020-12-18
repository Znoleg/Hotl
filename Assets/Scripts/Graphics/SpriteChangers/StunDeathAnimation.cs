using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StunDeathAnimation : CharacterGraphicsSpriteChanger
{
    private readonly EnemyGraphics _enemyGraphics;
    private readonly PlayerCharacter _player;
    private List<InStunSprites> _stunDeathSprites = new List<InStunSprites>();
    private InStunSprites _currentInUse;

    #region constructors

    public StunDeathAnimation(EnemyGraphics enemyGraphics, StunStatus startStatus = StunStatus.Floor) : base(enemyGraphics)
    {
        _enemyGraphics = enemyGraphics;
        _player = PlayerCharacter.Instance;
        _stunDeathSprites = _enemyGraphics.StunDeathSprites;

        ChangeDeathSprites(startStatus);
        
        _enemyGraphics.StunSetter.OnStatusChange += ChangeDeathSprites;
        _player.PlayerGraphics.StunKillAnimation.OnAnimStart += StartAnimation;
    }

    public static SpriteChangerBuilder<StunDeathAnimation> CreateBuilder(EnemyGraphics characterGraphics)
        => new SpriteChangerBuilder<StunDeathAnimation>(characterGraphics);

    #endregion

    public bool IsWorking { get; private set; }

    public event Action OnEnd;

    public override void Deinitialize()
    {
        base.Deinitialize();
        _player.PlayerGraphics.StunKillAnimation.OnAnimStart -= StartAnimation;
        _enemyGraphics.StunSetter.OnStatusChange -= ChangeDeathSprites;
    }

    private void StartAnimation()
    {
        IsWorking = true;
        Launch(cyclesAmount: 1, setFirstSpriteAfter: false, continueIfLaunched: true, () => { OnEnd?.Invoke(); IsWorking = false; });
    }

    private void ChangeDeathSprites(StunStatus status)
    {
        _currentInUse = _stunDeathSprites.FindConfomity(status);
        Sprites = _currentInUse.StatusSprites;
    }

}

