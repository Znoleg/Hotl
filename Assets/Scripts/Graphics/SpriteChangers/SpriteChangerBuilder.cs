using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SpriteChangerBuilder<T> where T : SpriteChangerBase
{
    private readonly T _spriteChanger;

    public SpriteChangerBuilder(MonoBehaviour behaviour)
    {
        _spriteChanger = (T)Activator.CreateInstance(
            typeof(T),
            BindingFlags.CreateInstance |
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.OptionalParamBinding,
            null,
            new[] { behaviour },
            CultureInfo.CurrentCulture);
    }

    public SpriteChangerBuilder<T> SetSprites(params Sprite[] sprites)
    {
        _spriteChanger.Sprites = sprites.ToList();
        return this;
    }

    public SpriteChangerBuilder<T> SetDelay(float delay)
    {
        _spriteChanger.Delay = delay;
        return this;
    }

    public SpriteChangerBuilder<T> SetFinishOnStop(bool finishOnStop)
    {
        _spriteChanger.FinishOnStop = finishOnStop;
        return this;
    }

    public T Load() => _spriteChanger;

    public static implicit operator T(SpriteChangerBuilder<T> builder) => builder.Load();
}
