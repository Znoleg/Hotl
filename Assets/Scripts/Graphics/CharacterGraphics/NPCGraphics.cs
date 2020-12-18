using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class InStunSprites
{
    public StunStatus StunStatus;
    public List<Sprite> StatusSprites = new List<Sprite>();
}


[System.Serializable]
public class CharacterSpriteContainer
{
    public Sprite ChillSprite;
    public GameObject CorpsePrefab;
    public List<InStunSprites> InStunSprites = new List<InStunSprites>();
    public List<GunSprites> GunSprites = new List<GunSprites>();
}

[Serializable]
public class AnimationStats
{
    public float _animDelay = 0.09f;
    public bool _finishOnStop = true;
}

[Serializable]
public class SpritesAnimationStats : AnimationStats
{
    [Space(8f)]

    public List<Sprite> _sprites = new List<Sprite>(); 
}

[Serializable]
public class LegAnimationStats : SpritesAnimationStats
{
    public SpriteRenderer _spriteRenderer;
}

[RequireComponent(typeof(SpriteRenderer))]
public abstract class NPCGraphics : MonoBehaviour, ISpriteChangerGroup
{
    [SerializeField] private SpriteRenderer _legsSpriteRenderer;
    [SerializeField] private NPCGraphicsContainer _npcGraphics;
    private Rigidbody2D _rigitBody;
    private CharacterCorpseSpawner _corpseSpawner;

    private readonly List<ISpriteChanger> _spriteChangers = new List<ISpriteChanger>();

    private readonly HashSet<IGroupedSpriteChanger> _groupedSpriteChangers = new HashSet<IGroupedSpriteChanger>();

    public Character HolderCharacter { get; protected set; }
    public GunSpriteHolder GunSpriteHolder { get; private set; }
    public CharacterSpriteContainer GraphicSprites => _npcGraphics.Graphics;
    public TorsoAnimation TorsoAnimation { get; private set; }
    public LegAnimation LegAnimation { get; private set; }
    protected NPCGraphicsContainer NPCGraphicsContainer => _npcGraphics;

    public void RegisterSpriteChanger(IGroupedSpriteChanger spriteChanger)
    {
        _groupedSpriteChangers.Add(spriteChanger);
        spriteChanger.SpriteChanging += StopConflictingSpriteChangers;
    }

    private void StopConflictingSpriteChangers(IGroupedSpriteChanger activeSpriteChanger)
    {
        var conflictingSpriteChangers = _groupedSpriteChangers
            .Where(spriteChanger => spriteChanger != activeSpriteChanger &&
            spriteChanger.SpriteRenderer == activeSpriteChanger.SpriteRenderer)
            .ToArray();
        foreach (var spriteChanger in conflictingSpriteChangers) spriteChanger.Stop();
    }

    protected virtual void Start()
    {
        //CheckGraphicSpritesFilled();
        if (!HolderCharacter) HolderCharacter = GetComponent<Character>();

        LegAnimation = AddSpriteChanger(() =>
            LegAnimation.CreateBuilder(this)
            .SetLegs(_legsSpriteRenderer)
            .SetSprites(_npcGraphics.LegAnimationStats._sprites.ToArray())
            .SetDelay(_npcGraphics.LegAnimationStats._animDelay)
            .SetFinishOnStop(_npcGraphics.LegAnimationStats._finishOnStop)
            .Load());

        GunSpriteHolder = AddSpriteChanger(() =>
            GunSpriteHolder.CreateBuilder(this)
            .SetDelay(_npcGraphics.GunHolderStats._animDelay)
            .SetFinishOnStop(_npcGraphics.GunHolderStats._finishOnStop)
            .Load());

        TorsoAnimation = AddSpriteChanger(() =>
            TorsoAnimation.CreateBuilder(this)
            .SetDelay(_npcGraphics.TorsoAnimationStats._animDelay)
            .SetFinishOnStop(_npcGraphics.TorsoAnimationStats._finishOnStop)
            .Load());

        _corpseSpawner = new CharacterCorpseSpawner(this, _npcGraphics.Graphics.CorpsePrefab); 
    }

    private void Update()
    {
        foreach (var spriteChanger in _spriteChangers) spriteChanger.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        foreach (var spriteChanger in _spriteChangers) spriteChanger.Deinitialize();
        _spriteChangers.Clear();

        foreach (var spriteChanger in _groupedSpriteChangers) spriteChanger.SpriteChanging -= StopConflictingSpriteChangers;
        _groupedSpriteChangers.Clear();

        _corpseSpawner.Deinitialize();
    }

    protected T AddSpriteChanger<T>(Func<T> creator) where T : ISpriteChanger
    {
        var spriteChanger = creator();
        _spriteChangers.Add(spriteChanger);
        return spriteChanger;
    }

    //private void CheckGraphicSpritesFilled()
    //{
    //    if (_npcGraphics.ChillSprite == null)
    //        throw new UnityException($"No Chill sprite in {gameObject.name} !");
    //    if (_npcGraphics.CorpsePrefab == null)
    //        throw new UnityException($"No Corpse prefab in {gameObject.name} !");
    //    if (_npcGraphics.GunSprites.Count == 0)
    //        throw new UnityException($"No Gun sprites in {gameObject.name} !");
    //    if (_npcGraphics.InStunSprites.Floor.Length == 0)
    //        throw new UnityException($"No Floor rise sprites in {gameObject.name} !");
    //    if (_npcGraphics.InStunSprites.Wall.Length == 0)
    //        throw new UnityException($"No Wall rise sprites in {gameObject.name} !");
    //}
}

