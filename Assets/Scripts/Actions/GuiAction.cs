using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GuiAction : InteractTarget
{
    [SerializeField] private Transform _guiObject;
    [SerializeField] private float _timer;
    [SerializeField] private GUIStyle _style;
    private bool _display;
    private const float _minuteScale = 60f;

    public override void DoAction()
    {
        _display = true;
    }

    private void Start()
    {
        if (!_guiObject) _guiObject = transform;
    }

    private void Update()
    {
        if (_display) _timer -= Time.deltaTime;
    }

    private void OnGUI()
    {
        if (!_display) return;
        GUI.depth = 0; //?
        var svMat = GUI.matrix;

        var width = Screen.width;
        var height = Screen.height;
        int minutes = (int)(_timer / _minuteScale);
        int seconds = (int)(_timer - minutes * _minuteScale); 
        var inScreenPos = Camera.main.WorldToScreenPoint(_guiObject.position);
        Debug.Log("Pos: " + inScreenPos);
        GUI.Box(new Rect(inScreenPos.x, inScreenPos.y - height / 2.5f, width / 3, height / 5), $"{minutes} : {seconds}", _style);

        GUI.matrix = svMat;
    }
}
