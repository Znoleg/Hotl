using System.Linq;
using UnityEngine;

public class TorsoAnimation : CharacterGraphicsSpriteChanger
{
    private readonly ObjectMovement _chMovement;
    private readonly Character _character;
    private bool _wasMoving;
    private GunSprites m_current;

    public TorsoAnimation(NPCGraphics characterGraphics) : base(characterGraphics)
    {
        _character = _characterGraphics.HolderCharacter;

        _character.OnGunPicked += SetWalkGunSprite;

        _chMovement = _character.BaseMovement != null ?
            _characterGraphics.HolderCharacter.BaseMovement :
            _characterGraphics.GetComponent<ObjectMovement>();
        
        if (!_chMovement)
            throw new UnityException($"No \"Movement\" script in {_characterGraphics.gameObject.name }!");
    }

    public static SpriteChangerBuilder<TorsoAnimation> CreateBuilder(NPCGraphics characterGraphics)
       => new SpriteChangerBuilder<TorsoAnimation>(characterGraphics);

    public override void Update(float deltaTime)
    {
        if (_chMovement.IsMoving && !_character.IsAttacking)
        {
            _wasMoving = true;
            Launch(continueIfLaunched: true);
        }
        else if (_wasMoving || _character.IsAttacking)
        {
            Stop();
        }
        base.Update(deltaTime);
    }

    private void SetWalkGunSprite()
    {
        if (m_current != null && _character.CurrentGun.GunType == m_current.gunType) return;
        m_current = _characterGraphics.GraphicSprites.GunSprites
            .First(gunSpr => gunSpr.gunType == _character.CurrentGun.GunType);
        Sprites = m_current.walkSprites.ToList();
        SetSprite(m_current.sprites[0]);
    }
}