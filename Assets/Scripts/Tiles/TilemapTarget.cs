using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap), typeof(TilemapRenderer), typeof(TilesChanger))]
public class TilemapTarget : Target
{
    //[SerializeField] private bool _infiniteAnimation;
    private TilesChanger _changeTiles;
    //private bool? _readyForNextAnim;

    protected override void Start()
    {
        base.Start();
        _changeTiles = GetComponent<TilesChanger>();
    }
    //protected override void Update()
    //{
    //    if (!_infiniteAnimation) return;

    //}
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
    }
    protected override bool DestroyEvent()
    {
        if (!base.DestroyEvent()) return false;
        _changeTiles.SwitchAllTiles3();
        StartCoroutine(Utils.WaitCoroutine(() => Transparent = true));
        return true;
        //StopAllCoroutines();
    }

    //private IEnumerator InfiniteAnimation()
    //{
    //    _changeTiles.SwitchAllTiles3();
    //}
}
