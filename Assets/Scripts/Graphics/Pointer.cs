using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] private Transform _pointer;
    [SerializeField] private float _rotation = 0f;
    private SpriteRenderer _sr;
    private TimelineManager _tm;
    private Vector3 _pointerStartPos;
    private Vector3 _upperPos;
    private Vector3 _downPos;
    private const float aboveValue = 1f;

    private void Start()
    {
        
        if (_pointer)
        {
            var thisPos = transform.position;
            _pointerStartPos = new Vector3(thisPos.x, thisPos.y + aboveValue, thisPos.z);
            _upperPos = _pointerStartPos + new Vector3(0f, aboveValue / 3f, 0f);
            _downPos = _pointerStartPos - new Vector3(0f, aboveValue / 3f, 0f);
            _pointer = Instantiate(_pointer, _upperPos, Quaternion.Euler(0f, 0f, _rotation), this.transform);
            _sr = _pointer.GetComponent<SpriteRenderer>();
            StartCoroutine(Utils.WaitCoroutine(() => _tm = TimelineManager.Instance));
            //_pointer.position = _upperPos;
        }
        
    }

    private void Update()
    {
        _pointer.position = new Vector3(_pointer.position.x, Mathf.PingPong(Time.time, _upperPos.y - _downPos.y) + _downPos.y,
            _pointer.position.z);
        if (_tm) _sr.enabled = !_tm.IsPlaying;
        //Debug.Log(Mathf.PingPong(Time.deltaTime, _upperPos.y) + _downPos.y);
    }
}
