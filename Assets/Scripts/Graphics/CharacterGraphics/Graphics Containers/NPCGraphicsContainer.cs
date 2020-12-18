using UnityEngine;

public abstract class NPCGraphicsContainer : ScriptableObject
{
    [SerializeField] private CharacterSpriteContainer _graphics;

    [Header("Animations stats")]
    [SerializeField] private AnimationStats _gunHolderStats;
    [SerializeField] private SpritesAnimationStats _torsoAnimationStats;
    [SerializeField] private SpritesAnimationStats _legAnimationStats;

    public CharacterSpriteContainer Graphics => _graphics;
    public AnimationStats GunHolderStats => _gunHolderStats;
    public SpritesAnimationStats TorsoAnimationStats => _torsoAnimationStats;
    public SpritesAnimationStats LegAnimationStats => _legAnimationStats;
}

