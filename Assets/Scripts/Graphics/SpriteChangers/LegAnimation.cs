using UnityEngine;

//[CreateAssetMenu(fileName = "New LegAnimation", menuName = "Animations/Leg Animation")]
public class LegAnimation : CharacterGraphicsSpriteChanger
{
    private readonly ObjectMovement m_chMovement;
    private bool m_wasMoving;
    private EnemyGraphics _enemyGraphics;

    #region Constructors

    public LegAnimation(NPCGraphics characterGraphics) : base(characterGraphics)
    {
        m_chMovement = _characterGraphics.HolderCharacter.BaseMovement != null ?
            _characterGraphics.HolderCharacter.BaseMovement :
            _characterGraphics.GetComponent<ObjectMovement>();
        if (!m_chMovement)
            throw new UnityException($"No \"Movement\" script in {_characterGraphics.gameObject.name }!");

        if (characterGraphics is EnemyGraphics enemyGraphics)
        {
            _enemyGraphics = enemyGraphics;
            enemyGraphics.StunSetter.OnStunStart += TurnOffLegs;
            enemyGraphics.StunSetter.OnStunEnd += TurnOnLegs;
        }
    }

    public static SpriteChangerBuilder<LegAnimation> CreateBuilder(NPCGraphics characterGraphics)
       => new SpriteChangerBuilder<LegAnimation>(characterGraphics);

    #endregion

    public SpriteRenderer Legs
    {
        get => SpriteRenderer;
        set => SpriteRenderer = value;
    }

    public override void Update(float deltaTime)
    {
        if (m_chMovement.IsMoving)
        {
            m_wasMoving = true;
            Launch(continueIfLaunched: true);
        }
        else if (m_wasMoving)
        {
            Stop();
            SetSprite(null);
        }
        base.Update(deltaTime);
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        if (_enemyGraphics)
        {
            _enemyGraphics.StunSetter.OnStunStart -= TurnOffLegs;
            _enemyGraphics.StunSetter.OnStunEnd -= TurnOnLegs;
        }
    }

    private void TurnOnLegs()
    {
        Legs.enabled = true;
    }

    private void TurnOffLegs()
    {
        Legs.enabled = false;
    }
}

public static class LegAnimationExtensions
{
    public static SpriteChangerBuilder<LegAnimation> SetLegs(
        this SpriteChangerBuilder<LegAnimation> builder,
        SpriteRenderer legs)
    {
        builder.Load().Legs = legs;
        return builder;
    }
}