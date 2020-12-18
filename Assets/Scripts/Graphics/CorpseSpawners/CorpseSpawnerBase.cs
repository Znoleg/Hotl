using UnityEngine;

public class CorpseSpawnerBase
{
    protected readonly MonoBehaviour _behaviour;
    protected Sprite _sprite;
    protected readonly GameObject _corpsePrefab;
    
    public CorpseSpawnerBase(MonoBehaviour behaviour, GameObject corpsePrefab)
    {
        _behaviour = behaviour;
        _corpsePrefab = corpsePrefab;
        SpriteRenderer = corpsePrefab.GetComponent<SpriteRenderer>();
    }

    protected GameObject SpawnedCorpse { get; private set; }
    protected SpriteRenderer SpriteRenderer { get; private set; }

    /// <summary>
    /// Creates new object with death sprite
    /// </summary>
    protected virtual void SpawnCorpse(float rotation, Sprite corpseSprite = null)
    {
        SpawnedCorpse = Object.Instantiate(_corpsePrefab, _behaviour.transform.position, Quaternion.identity);
        SpawnedCorpse.transform.eulerAngles = new Vector3(0f, 0f, rotation);
        
        SpriteRenderer = SpawnedCorpse.GetComponent<SpriteRenderer>();
        SpawnedCorpse.transform.position = _behaviour.transform.position;
        if (corpseSprite)
        {
            SpawnedCorpse.GetComponent<SpriteRenderer>().sprite = corpseSprite;
        }
    }
}
