using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum GunType
{
    Fists, Pipe, PoliceClub, Pistol, Machinegun, Shotgun
}

[Serializable]
public class GunSprites
{
    public GunType gunType;
    public Sprite[] walkSprites;
    public Sprite[] sprites;
    public float attackSprite;
}

public class GunSpriteHolder : CharacterGraphicsSpriteChanger
{
    private GunSprites m_current;
    private readonly Character _character;

    public GunSpriteHolder(NPCGraphics characterGraphics) : base(characterGraphics)
    {
        _character = _characterGraphics.HolderCharacter;
        if (_character.CurrentGun) SetHoldingGunSprite();
        _character.OnGunPicked += SetHoldingGunSprite;
        _character.OnCurrentGunShot += OnCurrentGunShot;
    }

    public static SpriteChangerBuilder<GunSpriteHolder> CreateBuilder(NPCGraphics characterGraphics)
        => new SpriteChangerBuilder<GunSpriteHolder>(characterGraphics);

    public bool IsCurrentSpriteIsAttack { get { return CurrentSpriteIndex == m_current.attackSprite; } }

    public void SetHoldingGunSprite()
    {
        if (m_current != null && _character.CurrentGun.GunType == m_current.gunType) return; // no need to set the same anim
        m_current = _characterGraphics.GraphicSprites.GunSprites
            .First(gunSpr => gunSpr.gunType == _character.CurrentGun.GunType);
        Sprites = m_current.sprites.ToList();
        SetSprite(m_current.sprites[0]);
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        _character.OnGunPicked -= SetHoldingGunSprite;
        _character.OnCurrentGunShot -= OnCurrentGunShot;
    }

    private void OnCurrentGunShot(Gun gun)
    {
        if (gun is MeleeGun)
        {
            Launch(cyclesAmount: 1, setFirstSpriteAfter: false, onComplete: FlipSprite);
        }
        else
        {
            Launch(cyclesAmount: 1, setFirstSpriteAfter: true);
        }
    }

    private void FlipSprite()
    {
        SpriteRenderer.flipY = !SpriteRenderer.flipY;
        ChangeSprite();
    }
}
