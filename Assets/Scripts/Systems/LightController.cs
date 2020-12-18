using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;

[System.Serializable]
public class MinMaxPair
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMaxPair(float min, float max)
    {
        Min = min;
        Max = max;
    }
}

public class LightController : MonoBehaviour
{
    [SerializeField] private MinMaxPair _LightON = new MinMaxPair(0f, 3f);
    [SerializeField] private MinMaxPair _LightOFF = new MinMaxPair(0f, 1f);
    private Light2D _light;
    private IEnumerator coro;
    private bool _started;

    private const float _OFFScale = 5f;

    private void Start()
    {
        _light = GetComponent<Light2D>();
    }

    private void Update()
    {
        if (!_started)
        {
            coro = OffOnCycle();
            StartCoroutine(coro);
        }
    }

    private IEnumerator OffOnCycle()
    {
        _started = true;

        var currentlyON = Random.Range(_LightON.Min, _LightON.Max);
        yield return new WaitForSeconds(currentlyON);
        _light.enabled = false;

        var currentlyOFF = Random.Range(_LightOFF.Min, _LightOFF.Max);
        yield return new WaitForSeconds(currentlyOFF);
        _light.enabled = true;

        _started = false;
    }
}
