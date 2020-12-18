using System;
using UnityEngine;

public interface IGroupedSpriteChanger : ISpriteChanger
{
    event Action<IGroupedSpriteChanger> SpriteChanging;

    SpriteRenderer SpriteRenderer { get; }

    void Stop();
}
