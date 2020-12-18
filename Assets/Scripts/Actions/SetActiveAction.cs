using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveAction : InteractTarget
{
    [SerializeField] private List<GameObject> _toSwitch;
    [SerializeField] private bool _statusToSwitch;

    public override void DoAction()
    {
        foreach (var toSwtch in _toSwitch) toSwtch.SetActive(_statusToSwitch);
    }
}
