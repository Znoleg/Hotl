using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : Gun
{
    [SerializeField] private Bullet _pfBullet;
    [SerializeField] private float _bulletLifeTime = 5f;
    [SerializeField] private float _bulletMoveSpeed = 50f;
    [SerializeField] private float _soundRange = 8f;
    [SerializeField] private AmmoType _ammoType;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _numberOfShots = 1f;
    [SerializeField] private float _scatterAngle = 0f;
    private Transform _transform;

    public float BulletLifeTime => _bulletLifeTime;
    public float BulletMoveSpeed => _bulletMoveSpeed;
    public float SoundRange => _soundRange;
    public Vector3 ShootPosition => _shootPoint.position;
    public AmmoType AmmoTyp => _ammoType;

    public override bool Shoot()
    {
        if (!base.Shoot()) return false;

        Vector2 directVector = _transform.right;
        if (_numberOfShots == 1f)
        {
            float rotation = Random.Range(-_scatterAngle, _scatterAngle);
            Vector2 bullerDir = directVector.Rotate(rotation);
            Bullet bullet = Instantiate(_pfBullet, _shootPoint.position, _transform.rotation);
            bullet.Launch(this, bullerDir, Damage, CurrentHolder);
        }

        else
        {
            List<Vector2> directions = new List<Vector2>();
            for (int i = 0; i < _numberOfShots; i++)
            {
                float rotation = Random.Range(0, _scatterAngle / 2);
                Vector2 rotatedVec = i % 2 == 0 ? directVector.Rotate(rotation) : directVector.Rotate(-rotation);
                directions.Add(rotatedVec);
            }

            foreach (var bulletDir in directions)
            {
                Bullet bullet = Instantiate(_pfBullet, _shootPoint.position, _transform.rotation);
                bullet.Launch(this, bulletDir, Damage, CurrentHolder);
            }
        }

        return true;
    }
    protected override void Start()
    {
        if (!_shootPoint)
            throw new UnityException($"{gameObject.name} has no ShootPoint");
        base.Start();
        _transform = transform;
    }
}
