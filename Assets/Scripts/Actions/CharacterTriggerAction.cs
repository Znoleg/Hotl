using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTriggerAction : InteractTarget
{
    [SerializeField] private Transform _target;
    [SerializeField] private List<NPCMovement> _toTrigger = new List<NPCMovement>();
    public override void DoAction()
    {
        foreach (NPCMovement nM in _toTrigger)
        {
            nM.AdditionalTrigger = true;
            nM.AIDestinationSetter.target = _target;
        }
    }

}
