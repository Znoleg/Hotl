using UnityEngine;

[CreateAssetMenu(fileName = "EnemyGraphics", menuName = "Graphics/Enemy", order = 1)]
public class EnemyGraphicsContainer : NPCGraphicsContainer
{
    [SerializeField] private AnimationStats _stunSetterStats;
    public AnimationStats StunSetterStats => _stunSetterStats;
}