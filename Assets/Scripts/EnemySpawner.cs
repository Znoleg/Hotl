using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : RepeatSpawner
{
    [Tooltip("Picks random gun from this array and sets to instantiated enemy")]
    [SerializeField] private Gun[] _gunsPrefab;
    private EnemyCharacter _enemy;
    
    //public EnemySpawner(GameObject prefab, Vector3 spawnPostiton, Vector3 spawnRotation)
    //    :base(prefab, spawnPostiton, spawnRotation) { }

    protected override void Start()
    {
        base.Start();
    }

    protected override void DoSpawn()
    {
        base.DoSpawn();
        _enemy = InstancedObject.GetComponent<EnemyCharacter>();
        if (!_enemy) return;
        StartCoroutine(SetEnemyReady());
    }

    private IEnumerator SetEnemyReady()
    {
        int gunNumber = (int)Random.Range(0f, _gunsPrefab.Length);
        var gun = Instantiate(_gunsPrefab[gunNumber]);
        gun.transform.SetParent(_enemy.transform, true);
        yield return new WaitForSeconds(0.01f);
        _enemy.EnemyMovement.AdditionalTrigger = true;
        _enemy.EnemyMovement.AIDestinationSetter.target = PlayerCharacter.Instance.transform;
        //_enemy.PickupGun(gun);
        _enemy.SpriteRenderer.sortingOrder = Prefab.GetComponent<SpriteRenderer>().sortingOrder;
    }

          
}
