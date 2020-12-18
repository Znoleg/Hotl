using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(fileName = "New Animated Tile", menuName = "Tiles/Changable Tile")]
public class ChangableTile : Tile
{
    [SerializeField] private Sprite[] m_Sprites;
    [SerializeField] private float m_AnimDelay;

    private int m_SpriteIndex = 0;

    public Sprite[] Sprites => m_Sprites;
    public Sprite firstSprite => m_Sprites[0];
    public float Delay => m_AnimDelay;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (m_Sprites != null && m_Sprites.Length > 0)
        {
            if (m_Sprites.Length <= m_SpriteIndex)
                m_SpriteIndex = 0;
            tileData.colliderType = colliderType;
            tileData.sprite = m_Sprites[m_SpriteIndex];
        }

    }

    public Sprite GetNextSprite()
    {
        if (m_Sprites.Length <= m_SpriteIndex)
            m_SpriteIndex = 0;
        return m_Sprites[m_SpriteIndex++];
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }

    public void SetIndexToMax()
    {
        m_SpriteIndex = m_Sprites.Length - 1;
    }
    public void SetIndexToMin()
    {
        m_SpriteIndex = 0;
    }
}
