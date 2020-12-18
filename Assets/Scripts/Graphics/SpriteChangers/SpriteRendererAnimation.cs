using UnityEngine;

public class AnimationSpriteChanger : SpriteChangerBase
{
    public AnimationSpriteChanger(MonoBehaviour behaviour) : base(behaviour)
    {

    }

    public static SpriteChangerBuilder<AnimationSpriteChanger> CreateBuilder(MonoBehaviour behaviour)
        => new SpriteChangerBuilder<AnimationSpriteChanger>(behaviour);

    public int? CyclesAmount { get; set; } = null;

    public void Animate()
    {
        Launch(cyclesAmount: CyclesAmount);
    }
}

public static class AnimationSpriteChangerExtensions
{
    public static SpriteChangerBuilder<AnimationSpriteChanger> SetCyclesAmount(
        this SpriteChangerBuilder<AnimationSpriteChanger> builder,
        int? cyclesAmount)
    {
        builder.Load().CyclesAmount = cyclesAmount;
        return builder;
    }
}

public class SpriteRendererAnimation : MonoBehaviour
{
    [SerializeField] private bool _animatedOnStart = true;
    [SerializeField] private int? _cyclesAmount = null;

    private AnimationSpriteChanger _spriteChanger;

    public AnimationSpriteChanger AnimationSpriteChanger => _spriteChanger;

    private void Start()
    {
        _spriteChanger = AnimationSpriteChanger.CreateBuilder(this).SetCyclesAmount(_cyclesAmount);
        if (_animatedOnStart) _spriteChanger.Animate();
    }

    private void Update()
    {
        _spriteChanger.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _spriteChanger.Deinitialize();
    }
}
