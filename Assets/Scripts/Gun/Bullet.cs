using System.Collections;
using UnityEngine;

public class Bullet : Damager
{
    private float lifeTime;
    private float moveSpeed;

    private IEnumerator lifeCycle;
    private Vector3 shootDir;

    public void Launch(ProjectileGun gun, Vector2 direction, float damage, Character shooter)
    {
        if (!gun) return;
        lifeTime = gun.BulletLifeTime;
        shootDir = direction;
        moveSpeed = gun.BulletMoveSpeed;
        Damage = damage;
        lifeCycle = LifeCycle();
        StartCoroutine(lifeCycle);
        Begin(shooter);
    }

    private void Update()
    {
        Move();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!DamageHolderCheck(collision)) return;
        if (collision.GetComponent<PhysicalBody>())
        {
            Destroy(gameObject);
        }
    }

    // if shootDir not initialized (Launch() method not called) movement will be 0
    private void Move()
    {
        Vector3 moveVec = shootDir * moveSpeed * Time.deltaTime;
        transform.position += moveVec;
    }

    private IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
        DestroyCourutine();
    }

    private void DestroyCourutine()
    {
        if (lifeCycle != null)
        {
            StopCoroutine(lifeCycle);
            lifeCycle = null;
        }
    }
}
