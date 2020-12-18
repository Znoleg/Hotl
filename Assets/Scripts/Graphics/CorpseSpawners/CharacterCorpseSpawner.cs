using UnityEngine;

public class CharacterCorpseSpawner : CorpseSpawnerBase
{
    private readonly NPCGraphics _characterGraphics;
    private Sprite _currentDeathSprite;
    
    public CharacterCorpseSpawner(NPCGraphics characterGraphics, GameObject corpsePrefab)
        : base(characterGraphics, corpsePrefab)
    {
        _characterGraphics = characterGraphics;
        _characterGraphics.HolderCharacter.OnDeathAction += SpawnProperCorpse;
    }

    private void SpawnProperCorpse(float zRotation)
    {
        ChangeSprite();
        SpawnCorpse(zRotation, _currentDeathSprite);
        SpawnedCorpse.transform.SetParent(LevelLoader.CurrentLevel.transform, true);
        SpawnedCorpse.GetComponent<Rigidbody2D>().PushBody(500f, -SpawnedCorpse.transform.right, ForceMode2D.Force);
    }

    public void Deinitialize()
    {
        _characterGraphics.HolderCharacter.OnDeathAction -= SpawnProperCorpse;
    }

    private void ChangeSprite()
    {
        if (_characterGraphics is EnemyGraphics enemyGraphics)
        {
            var stunSetter = enemyGraphics.StunSetter;
            if (stunSetter.IsWorking)
            {
                var sprites = enemyGraphics.StunDeathAnimation.Sprites;
                _currentDeathSprite = sprites[sprites.Count - 1];
            }
        }
        else _currentDeathSprite = _corpsePrefab.GetComponent<SpriteRenderer>().sprite; // pretty bad
    }

}