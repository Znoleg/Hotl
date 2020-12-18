using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPositionHolder : MonoBehaviour
{
    [SerializeField] private List<UIBehaviour> _UIToHold = new List<UIBehaviour>();
    private bool _hold;

    public void Launch()
    {
        _hold = true;
    }

    public void Launch(Vector2 position)
    {
        SetPosition(position);
        _hold = true;
    }

    public void Stop()
    {
        _hold = false;
    }

    private void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    private void Update()
    {
        if (_hold)
        {
            foreach (UIBehaviour ui in _UIToHold)
                ui.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }
}

//public class UIPositionHolder : MonoBehaviour
//{
//    [SerializeField] private List<UIBehaviour> _UIToHold;
//    private bool _hold = true;

//    private void Start()
//    {
//        _hold = true;
//        _UIToHold.OnTimerStop += () => _hold = false;
//    }

//    private void Update()
//    {
//        if (_hold) _UIToHold.transform.position = Camera.main.WorldToScreenPoint(transform.position);
//    }
//}
