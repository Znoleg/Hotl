using System;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    private HingeDoor _hingeDoor;
    private const float _closeCooldown = 3f;
    private CoroutineObject _holdOpenedDoorRoutine;

    public event Action OnPlayerTouch;

    private void Awake()
    {
        _holdOpenedDoorRoutine = new CoroutineObject(this, HoldOpenedDoor);
    }

    private void Start()
    {
        _hingeDoor = GetComponentInParent<HingeDoor>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        var character = collision.transform.GetComponent<Character>();
        if (character is EnemyCharacter enemy && _hingeDoor.CanHitEnemy)
        {
            enemy.ChangeStatus(NPCStatus.Stunned);
            enemy.SetHiterDoor();

            return;
        }
        else if (character is PlayerCharacter)
        {
            OnPlayerTouch?.Invoke();
        }

        if (_hingeDoor.IsClosing) return;

        if (_hingeDoor.Angle > _hingeDoor.CloseLimits.min)
        {
            _hingeDoor.LockDoor(HingeDoor.Limit.Positive);
        }
        else if (_hingeDoor.Angle < _hingeDoor.CloseLimits.min)
        {
            _hingeDoor.LockDoor(HingeDoor.Limit.Negative);
        }

        _holdOpenedDoorRoutine.Start();
    }

    private IEnumerator HoldOpenedDoor()
    {
        yield return new WaitForSeconds(_closeCooldown);
        _hingeDoor.CloseDoorOnce();
        
        _holdOpenedDoorRoutine.Stop();
        yield break;
    }
}
