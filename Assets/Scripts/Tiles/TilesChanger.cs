using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapRenderer), typeof(Rigidbody2D), typeof(TilemapTarget))]
public class TilesChanger : MonoBehaviour
{
    private Tilemap m_tilemap;
    private Dictionary<Vector3Int, ChangableTile> m_PosTilePairs = new Dictionary<Vector3Int, ChangableTile>();
    private float _spriteNum = 0f;

    public float CurrentSpriteIndex => _spriteNum;

    void Start()
    {
        m_tilemap = GetComponent<Tilemap>();
    }

    public void SwitchTile()
    {
        BoundsInt bounds = m_tilemap.cellBounds;
        bool updated = false;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = m_tilemap.GetTile(pos);

            if (tile && tile is ChangableTile changableTile)
            {
                if (!updated)
                {
                    Sprite sprite = changableTile.GetNextSprite();
                    changableTile.sprite = sprite;
                    updated = true;
                }
                m_tilemap.RefreshTile(pos);
            }
        }
    }

    public IEnumerator SwitchAllTiles()
    {
        BoundsInt bounds = m_tilemap.cellBounds;

        bool spritesLeft = true;

        while (spritesLeft)
        {
            bool updated = false;
            float? delay = null;

            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                TileBase tile = m_tilemap.GetTile(pos);

                if (tile && tile is ChangableTile changableTile && _spriteNum < changableTile.Sprites.Length - 1)
                {
                    if (!m_PosTilePairs.ContainsKey(pos)) m_PosTilePairs.Add(pos, changableTile);
                    if (!delay.HasValue || changableTile.Delay > delay.Value) delay = changableTile.Delay;
                    if (!updated)
                    {
                        var sprite = changableTile.GetNextSprite();
                        changableTile.sprite = sprite;
                        updated = true;
                    }
                    m_tilemap.RefreshTile(pos);
                }
            }

            if (delay.HasValue)
            {
                yield return new WaitForSeconds(delay.Value);
                _spriteNum++;
            }
            else
            {
                spritesLeft = false;
                _spriteNum = 0;
            }
        }
    }

    public void SwitchAllTiles3()
    {
        StartCoroutine(SwitchAllTiles());
    }

    private void OnApplicationQuit()
    {
        if (m_PosTilePairs != null && m_PosTilePairs.Count > 0)
        {
            foreach (KeyValuePair<Vector3Int, ChangableTile> PosTiles in m_PosTilePairs)
            {
                ChangableTile changableTile = PosTiles.Value;
                changableTile.SetIndexToMin();              
            }
            m_tilemap.RefreshAllTiles();
        }
    }
}