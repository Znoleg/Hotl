using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] private Material _material;
    private CoroutineObject _changeEmissionRoutine;

    private void Start()
    {
        if (!_material) _material = GetComponent<SpriteRenderer>().material;
    }

    public void ChangeEmissionColor(int intensity, Color? color = null)
    {
        var clr = color.HasValue ? color.Value : _material.color;
        _material.SetColor("_Emission", clr * intensity);
    }

    public void ChangeEmissionColor(float intensityBegin, float intensityEnd, float deltaSeconds, Color? color = null)
    {

    }

    //public void ChangeEmissionColor(float deltaIntensity, float deltaSeconds, Color? color = null)
    //{

    //}

    private IEnumerator ChangeEmissionSequentially(float intensityBegin, float intensityEnd, float deltaSeconds, Color color)
    {
        var changeTick = intensityEnd > intensityBegin ? 1 : -1;

        while (intensityBegin != intensityEnd)
        {
            yield return new WaitForSeconds(deltaSeconds);
            intensityBegin += changeTick;
        }
    }

}
