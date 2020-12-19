using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGraphics : NPCGraphics
{
    [SerializeField] private List<InStunSprites> _stunDeathSprites = new List<InStunSprites>();
    [SerializeField] private AnimationStats _stunDeathAnimStats;

    public StunSetter StunSetter { get; private set; }
    public List<InStunSprites> StunDeathSprites => _stunDeathSprites;
    public StunDeathAnimation StunDeathAnimation { get; private set; }
    public EnemyCharacter HolderEnemy { get; private set; }

    protected override void Start()
    {
        HolderCharacter = GetComponent<Character>();
        HolderEnemy = GetComponent<EnemyCharacter>(); // pretty bad
        
        EnemyGraphicsContainer enemyGraphics = null; // very bad
        if (NPCGraphicsContainer is EnemyGraphicsContainer eG) enemyGraphics = eG;

        StunSetter = AddSpriteChanger(() =>
            StunSetter.CreateBuilder(this)
            .SetDelay(enemyGraphics.StunSetterStats._animDelay)
            .SetFinishOnStop(enemyGraphics.StunSetterStats._finishOnStop)
            .Load());

        base.Start();
        
        StunDeathAnimation = AddSpriteChanger(() =>
            StunDeathAnimation.CreateBuilder(this)
            .SetDelay(_stunDeathAnimStats._animDelay)
            .SetFinishOnStop(_stunDeathAnimStats._finishOnStop)
            .Load());
    }
}

