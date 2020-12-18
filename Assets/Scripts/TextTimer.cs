using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTimer : MonoBehaviour
{
    [SerializeField] private float _timer;
    private Text _text;
    private const float _minuteScale = 60f;
    private int _minutes;
    private int _seconds;
    private string _timeText;

    public delegate void TimerStopAction();

    public event TimerStopAction OnTimerStop;

    private void Start()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            _timer -= Time.deltaTime;
            _minutes = (int)(_timer / _minuteScale);
            _seconds = (int)(_timer - _minutes * _minuteScale);
            if (_seconds / 10 == 0) _timeText = $"{_minutes} : 0{_seconds}";
            else _timeText = $"{_minutes} : {_seconds}"; 
            _text.text = _timeText;

            if (_timer <= 0f)
            {
                OnTimerStop?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
