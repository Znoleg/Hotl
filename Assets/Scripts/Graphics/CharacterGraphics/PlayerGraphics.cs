using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class StunKillSprites : InStunSprites
{
    public Sound KillSound;
    public float SoundStartDelay = 0f;
}

public class PlayerGraphics : NPCGraphics
{
    [SerializeField] private List<StunKillSprites> _stunKillSprites = new List<StunKillSprites>();
    [SerializeField] private AnimationStats _stunKillAnimationStats;
    public List<StunKillSprites> StunKillSprites => _stunKillSprites;

    public StunKillAnimation StunKillAnimation { get; private set; }

    protected override void Start()
    {
        foreach (StunKillSprites stunKillSprites in _stunKillSprites)
        {
            AudioManager.Instance.InitSound(stunKillSprites.KillSound);
        }

        base.Start();
        StunKillAnimation = AddSpriteChanger(() =>
            StunKillAnimation.CreateBuilder(this)
            .SetDelay(_stunKillAnimationStats._animDelay)
            .SetFinishOnStop(_stunKillAnimationStats._finishOnStop)
            .Load());
        
        StunKillAnimation.OnStunKillEnd += SetFirstSprite;
    }

    private void SetFirstSprite()
    {
        GunSpriteHolder.SetSprite(GunSpriteHolder.Sprites[0]);
    }

    private void OnDestroy()
    {
        StunKillAnimation.OnStunKillEnd -= SetFirstSprite;
    }
}
