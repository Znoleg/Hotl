using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vector2Percent
{
    [Range(0.0f, 100.0f)]
    public float X, Y;
}

[RequireComponent(typeof(SpriteRendererAnimation))]
public class Window : DestroyableTarget
{
    [SerializeField] private Vector2 _colliderOffsetAfterHit;
    [SerializeField] private Vector2 _colliderSizeAfterHit;
    private SpriteRendererAnimation _animation;
    private Sprite _brokenParticles;
    private bool _broken;

    protected override void Start()
    {
        base.Start();
        _animation = GetComponent<SpriteRendererAnimation>();
    }

    public override void TakeDamage(float amount)
    {
        if (_broken) return;
        _broken = true;
        Transparent = true;
        Collider.offset = _colliderOffsetAfterHit;
        if (Collider is BoxCollider2D boxCollider) boxCollider.size = _colliderSizeAfterHit;
        _animation.AnimationSpriteChanger.Animate();
    }

    protected override bool DestroyEvent()
    {
        return false;
    }
}
