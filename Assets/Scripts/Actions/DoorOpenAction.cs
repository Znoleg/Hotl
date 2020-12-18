using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenDoor", menuName = "InteractableAction/OpenDoor")]
public class DoorOpenAction : InteractTarget 
{
    [SerializeField] private List<HingeDoor> _doors;

    public override void DoAction()
    {
        foreach (HingeDoor door in _doors)
            door.UnlockDoor();
    }
}
