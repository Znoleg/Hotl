using System;
using UnityEngine;

//public abstract class CharacterGraphicsSpriteChanger : SpriteChangerBase
//{
//    protected readonly CharacterGraphics _characterGraphics;

//    public CharacterGraphicsSpriteChanger(CharacterGraphics characterGraphics, float animDelay = 0.09f, bool finishOnStop = true) :
//        base(characterGraphics, animDelay, finishOnStop)
//    {
//        _characterGraphics = characterGraphics;
//    }

//    public CharacterGraphicsSpriteChanger(CharacterGraphics characterGraphics, Sprite[] sprites, float animDelay = 0.09f, bool finishOnStop = true) :
//        base(characterGraphics, sprites, animDelay, finishOnStop)
//    {
//        _characterGraphics = characterGraphics;
//    }
//}

public abstract class GroupedSpriteChangerBase<T> : SpriteChangerBase, IGroupedSpriteChanger where T : MonoBehaviour, ISpriteChangerGroup
{
    public GroupedSpriteChangerBase(T behaviour) : base(behaviour)
    {
        behaviour.RegisterSpriteChanger(this);
    }

    public event Action<IGroupedSpriteChanger> SpriteChanging;

    SpriteRenderer IGroupedSpriteChanger.SpriteRenderer => SpriteRenderer;

    public void Stop() => Stop(finish: false);

    public override void Launch(
        int? cyclesAmount = null,
        bool setFirstSpriteAfter = false,
        bool continueIfLaunched = false,
        Action onComplete = null)
    {
        SpriteChanging?.Invoke(this);
        base.Launch(cyclesAmount, setFirstSpriteAfter, continueIfLaunched, onComplete);
    }

    public override void SetSprite(Sprite sprite)
    {
        SpriteChanging?.Invoke(this);
        base.SetSprite(sprite);
    }

    protected override void ChangeSprite()
    {
        SpriteChanging?.Invoke(this);
        base.ChangeSprite();
    }
}

public abstract class CharacterGraphicsSpriteChanger : GroupedSpriteChangerBase<NPCGraphics>
{
    protected readonly NPCGraphics _characterGraphics;

    public CharacterGraphicsSpriteChanger(NPCGraphics characterGraphics) : base(characterGraphics)
    {
        _characterGraphics = characterGraphics;
    }
}