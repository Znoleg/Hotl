using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBGTransfusion : MonoBehaviour
{
    [SerializeField] private Color[] _colorsToLerp;
    [SerializeField] private float _changeSpeed;
    private Camera _camera;

    private CoroutineObject _colorsRoutine;
    private Action OnRoutineEnd;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _colorsRoutine = new CoroutineObject(this, DoColorChangeCycle);
        OnRoutineEnd += StartChangeColors;
    }

    private void Start()
    {
        _colorsRoutine.Start();
    }

    private void StartChangeColors()
    {
        _colorsRoutine.Start();
    }

    private IEnumerator DoColorChangeCycle()
    {
        foreach (Color color in _colorsToLerp)
        {
            while (!CheckColorsEqual(_camera.backgroundColor, color))
            {
                Color currentColor = _camera.backgroundColor;
                _camera.backgroundColor = Color.Lerp(currentColor, color, _changeSpeed * Time.deltaTime);

                yield return null;
            }
        }

        _colorsRoutine.Stop();
        OnRoutineEnd.Invoke();
        yield break;
    }

    private bool CheckColorsEqual(Color color1, Color color2)
    {
        return Math.Round(color1.r, 3) == Math.Round(color2.r, 3)
            && Math.Round(color1.g, 3) == Math.Round(color2.g, 3)
            && Math.Round(color1.b, 3) == Math.Round(color2.b, 3);
    }

    private void OnDestroy()
    {
        OnRoutineEnd -= StartChangeColors;
    }
}
