using Assets.Scripts.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _instancePrefab;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] protected Vector3 _spawnRotation = Vector3.zero;

    public GameObject InstancedObject { get; private set; }

    public GameObject Prefab => _instancePrefab;

    public Vector3 Position => _spawnPosition.position;

    public Vector3 Rotation => _spawnRotation;


    public void Spawn(GameObject prefab, Vector2 position, Vector2 rotation)
    {
        _instancePrefab = prefab;
        _spawnPosition.position = position;
        _spawnRotation = rotation;
        DoSpawn();
    }
    protected virtual void Start()
    {
        if (!_spawnPosition) _spawnPosition = transform;
    }

    protected virtual void DoSpawn()
    {
        InstancedObject = Instantiate(Prefab, Position, Quaternion.LookRotation(Rotation));
        //obj.transform.LookAt(_toLookAt);
    }
}

public class RepeatSpawner : Spawner
{
    [SerializeField] private float _minSpawnDelay = 2.0f;
    [SerializeField] private float _maxSpawnDelay = 7.5f;
    private bool _readyToSpawn = true;

    //public RepeatSpawner(GameObject prefab, Vector3 spawnPositon, Vector3 spawnRotation)
    //    : base(prefab, spawnPositon, spawnRotation) { }

    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay));
        DoSpawn();
    }

    protected override void DoSpawn()
    {
        base.DoSpawn();
        _readyToSpawn = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        if (_readyToSpawn)
        {
            _readyToSpawn = false;
            StartCoroutine(SpawnCoroutine());
        }
    }

}
